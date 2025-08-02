using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using WorklogManagement.AI;
using WorklogManagement.AI.Helpers;

#if DEBUG
Console.Title = "WorklogManagement.AI";
#endif

// ─────────────────────────────────────────────
// Konfiguration
// ─────────────────────────────────────────────

const string baseUrl = "https://localhost:7182";

var apiReady = await WaitForApiReadyAsync($"{baseUrl}/health");
if (!apiReady)
{
    Console.WriteLine("API nicht erreichbar.");
    return;
}

var routes = await SwaggerHelper.GetGetRoutesAsync($"{baseUrl}/swagger/v1/swagger.json");
var routeList = string.Join(Environment.NewLine, routes.Select(r => $"GET {r}"));

var modelDescriptions = ModelDescriptionHelper.GenerateModelDescriptions("WorklogManagement.Data", "WorklogManagement.Data.Models");
var enumDescriptions = ModelDescriptionHelper.GenerateEnumDescriptions("WorklogManagement.Shared");

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

var classifierSystemPrompt = $"""
Du bist ein Assistent zur Klassifikation von Benutzeranfragen.

Ziel ist es zu erkennen, ob der Benutzer Informationen aus einer Datenquelle (API) abrufen möchte.

Antworte ausschließlich mit einem der folgenden Begriffe:
- DATENABFRAGE
- SONSTIGES

Zur Beurteilung kannst du dich auf folgende Datenstruktur stützen:
{dataModeling}

Wenn du dir unsicher bist, wähle bitte SONSTIGES.
""";

var urlSystemPrompt = $"""
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

var answerSystemPrompt = $"""
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

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = config["OpenAI:ApiKey"]
    ?? throw new InvalidOperationException("API-Key nicht gefunden!");

// ─────────────────────────────────────────────
// Semantic Kernel Setup
// ─────────────────────────────────────────────

ChatHistory classifierHistory = new(classifierSystemPrompt);
ChatHistory urlHistory = new(urlSystemPrompt);
ChatHistory answerHistory = new(answerSystemPrompt);

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(modelId: "gpt-4o", apiKey: apiKey, serviceId: "openai")
    .Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();

using HttpClient http = new();

while (true)
{
    // ─────────────────────────────────────────────
    // Benutzerabfrage & GPT-Generierung
    // ─────────────────────────────────────────────

    Console.Write("Anfrage: ");
    var request = Console.ReadLine()!;

    classifierHistory.AddUserMessage(request);
    var classification = await chat.GetChatMessageContentAsync(classifierHistory);

    if (classification.Content!.Equals("DATENABFRAGE", StringComparison.OrdinalIgnoreCase))
    {
        urlHistory.AddUserMessage(request);
        var response = await chat.GetChatMessageContentAsync(urlHistory);

        var urls = response.Content!
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .ToArray();

        // ─────────────────────────────────────────────
        // API-Aufruf & Analyse
        // ─────────────────────────────────────────────

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

        var originalRequest = request;
        request = $"""
            Analysiere die folgenden JSON-Daten:
            [{string.Join(',', jsonList)}]

            Gib eine zusammengefasste Antwort auf die ursprüngliche Anfrage:
            {originalRequest}
            """;
    }

    answerHistory.AddUserMessage(request);
    var answer = await chat.GetChatMessageContentAsync(answerHistory);

    Console.Write("Antwort: ");
    Console.WriteLine(answer.Content);
    Console.WriteLine();
}

static async Task<bool> WaitForApiReadyAsync(string url, int maxRetries = 10)
{
    using HttpClient http = new();

    for (var i = 0; i < maxRetries; i++)
    {
        try
        {
            var res = await http.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch
        {
            // Ignorieren – API ist wohl noch nicht bereit
        }

        await Task.Delay(1000);
    }

    return false;
}
