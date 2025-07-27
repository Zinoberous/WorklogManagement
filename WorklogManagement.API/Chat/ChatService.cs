using Microsoft.SemanticKernel.ChatCompletion;

namespace WorklogManagement.API.Chat;

public interface IChatService
{
    Task<Answer> AskAsync(Request request, string baseUrl, CancellationToken cancellationToken = default);
}

public class ChatService(
    IChatCompletionService chat,
    IHttpClientFactory httpClientFactory,
    ISwaggerService swaggerService,
    IModelDescriptionService modelDescriptionService)
    : IChatService
{
    private Guid? _currentChatId;

    private bool _isSystemPromptsInitialized = false;
    private string _classifierSystemPrompt = string.Empty;
    private string _urlSystemPrompt = string.Empty;
    private string _answerSystemPrompt = string.Empty;

    private ChatHistory _classifierChatHistory = [];
    private ChatHistory _urlChatHistory = [];
    private ChatHistory _answerChatHistory = [];

    public async Task<Answer> AskAsync(Request request, string baseUrl, CancellationToken cancellationToken = default)
    {
        if (_currentChatId is null || _currentChatId != request.ChatId)
        {
            _currentChatId = request.ChatId ?? Guid.NewGuid();

            await InitializeSystemPromptsAsync(baseUrl, cancellationToken);

            _classifierChatHistory = new(_classifierSystemPrompt);
            _urlChatHistory = new(_urlSystemPrompt);
            _answerChatHistory = new(_answerSystemPrompt);
        }

        var prompt = request.Prompt;

        _classifierChatHistory.AddUserMessage(prompt);
        var classification = await chat.GetChatMessageContentAsync(_classifierChatHistory, cancellationToken: cancellationToken);

        if (classification.Content!.Equals("DATENABFRAGE", StringComparison.OrdinalIgnoreCase))
        {
            using var http = httpClientFactory.CreateClient();

            _urlChatHistory.AddUserMessage(prompt);
            var response = await chat.GetChatMessageContentAsync(_urlChatHistory, cancellationToken: cancellationToken);

            var urls = response.Content!
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .ToArray();

            var tasks = urls.Select(async url =>
            {
                try
                {
                    var response = await http.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Fehler: {url} - {ex.Message}");
                    return null;
                }
            });

            var jsonList = (await Task.WhenAll(tasks)).Where(json => json != null).ToArray();

            var originalPrompt = prompt;
            prompt = $"""
Analysiere die folgenden JSON-Daten:
[{string.Join(',', jsonList)}]

Gib eine zusammengefasste Antwort auf die ursprüngliche Anfrage:
{originalPrompt}
""";
        }

        _answerChatHistory.AddUserMessage(prompt);
        var answer = await chat.GetChatMessageContentAsync(_answerChatHistory, cancellationToken: cancellationToken);

        return new()
        {
            ChatId = _currentChatId.Value,
            Message = answer.Content!,
        };
    }

    private async Task InitializeSystemPromptsAsync(string baseUrl, CancellationToken cancellationToken)
    {
        if (_isSystemPromptsInitialized)
        {
            return;
        }

        var routes = await swaggerService.GetGetRoutesAsync($"{baseUrl}/swagger/v1/swagger.json", cancellationToken);
        var routeList = string.Join(Environment.NewLine, routes.Select(r => $"GET {r}"));

        var modelDescriptions = modelDescriptionService.GenerateModelDescriptions("WorklogManagement.Data", "WorklogManagement.Data.Models");
        var enumDescriptions = modelDescriptionService.GenerateEnumDescriptions("WorklogManagement.Shared");

        var dataModeling = $"""
Basis-URL für APIs:
{baseUrl}

Endpunkte:
{routeList}

Query-Parameter:
- filter: LINQ-Ausdruck basierend auf den tatsächlichen Spaltennamen
- sortBy: Spaltenname mit optionalem Suffix ` desc` für absteigende Sortierung (z. B. `CreatedAt desc`).
- pageSize: Anzahl Ergebnisse. 0 = alle.
- pageIndex: 0-basiert. 0 = erste Seite.

Modelle:
{string.Join(Environment.NewLine, modelDescriptions)}

Enums:
{string.Join(Environment.NewLine, enumDescriptions)}
""";

        _classifierSystemPrompt = $"""
Du bist ein Assistent zur Klassifikation von Benutzeranfragen.

Ziel ist es zu erkennen, ob der Benutzer Informationen aus einer Datenquelle (API) abrufen möchte.

Antworte ausschließlich mit einem der folgenden Begriffe:
- DATENABFRAGE
- SONSTIGES

Zur Beurteilung kannst du dich auf folgende Datenstruktur stützen:
{dataModeling}

Wenn du dir unsicher bist, wähle bitte SONSTIGES.
""";

        _urlSystemPrompt = $"""
Du bist ein API-URL-Generator.

Ziel ist es, anhand der Benutzeranfrage möglichst passende GET-Endpunkte mit vollständiger URL inklusive Query-Parameter zu generieren, die die Anfrage beantworten könnten.

Folgende Informationen stehen dir zur Verfügung:
{dataModeling}

Vorgehen:
1. Interpretiere die Benutzeranfrage.
2. Bestimme die relevanten GET-Endpunkte.
3. Generiere **eine oder mehrere vollständige URLs** mit sinnvollen Parametern, um passende Daten abzufragen.

Rückgabe:
- Gib **ausschließlich die vollständigen URLs** (eine pro Zeile) zurück.
- Kein Kommentar, keine Erklärung.
""";

        _answerSystemPrompt = $"""
Du bist ein Assistent zur Analyse von JSON-Daten oder zur direkten Beantwortung allgemeiner Anfragen.

Wenn dir JSON-Daten übergeben werden:
- Analysiere die Daten im Kontext der ursprünglichen Anfrage.
- Gib prägnant und verständlich wieder, was sie bedeuten.
- Vermeide technische Begriffe, wenn möglich.
- Fasse zusammen, vergleiche, erkenne Muster oder Besonderheiten.

Wenn dir keine JSON-Daten vorliegen:
- Beantworte die Benutzeranfrage in natürlicher Sprache.
- Nutze dein Wissen über die zugrunde liegende API und Datenstruktur:
{dataModeling}
""";

        _isSystemPromptsInitialized = true;
    }
}
