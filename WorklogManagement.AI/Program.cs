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

var routes = await OpenApiHelper.GetGetRoutesAsync($"{baseUrl}/swagger/v1/swagger.json");
var routeList = string.Join("\n", routes.Select(r => $"GET {r}"));

var modelDescriptions = ModelDescriptionHelper.GenerateModelDescriptions("WorklogManagement.Data", "WorklogManagement.Data.Models");
var enumDescriptions = ModelDescriptionHelper.GenerateEnumDescriptions("WorklogManagement.Shared");

var systemPrompt = $"""
Du bist ein API-Assistent.

Die API ist unter folgender Basis-URL erreichbar:
{baseUrl}

Folgende GET-Endpunkte stehen zur Verfügung:
{routeList}

Query-Parameter:
- `filter`: LINQ-Ausdruck basierend auf den tatsächlichen Spaltennamen
- `sortBy`: Spaltenname mit optionalem Suffix ` desc` für absteigende Sortierung (z. B. `CreatedAt desc`).
- `pageSize`: Anzahl Ergebnisse. 0 = alle.
- `pageIndex`: 0-basiert. 0 = erste Seite.

Modelle:
{string.Join(Environment.NewLine, modelDescriptions)}

Enums:
{string.Join(Environment.NewLine, enumDescriptions)}

Antwort: 
1. Wenn die Frage eine Datenabfrage ist (z. B. "Zeige alle Tickets mit Status 'Todo'"), generiere eine vollständige GET-URL basierend auf den verfügbaren Endpunkten und Parametern. Gib ausschließlich die URL zurück. Keine Kommentare.
2. Wenn JSON-Daten zur Analyse bereitgestellt werden, analysiere die Daten und beantworte die Frage.
3. Wenn die Frage keine Datenabfrage ist (z. B. "Was ist ein Ticket?"), antworte direkt und präzise in natürlicher Sprache.
4. Wenn du unsicher bist, ob es sich um eine Datenabfrage handelt, frage den Benutzer zur Klärung nach.
""";

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = config["OpenAI__ApiKey"]
    ?? throw new InvalidOperationException("API-Key nicht gefunden!");

// ─────────────────────────────────────────────
// Semantic Kernel Setup
// ─────────────────────────────────────────────

var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(
    modelId: "gpt-4o",
    apiKey: apiKey,
    serviceId: "openai"
);

var kernel = builder.Build();
var chat = kernel.GetRequiredService<IChatCompletionService>();

while (true)
{
    // ─────────────────────────────────────────────
    // Benutzerabfrage & GPT-Generierung
    // ─────────────────────────────────────────────

    Console.Write("Frage: ");
    var question = Console.ReadLine();

    ChatHistory chatHistory = new(systemPrompt);
    chatHistory.AddUserMessage(question!);

    var gptResponse = await chat.GetChatMessageContentAsync(chatHistory);
    if (!Uri.TryCreate(gptResponse.Content, UriKind.Absolute, out var generatedUrl))
    {
        Console.Write("Antwort: ");
        Console.WriteLine(gptResponse.Content);
        Console.WriteLine();

        continue;
    }

    // ─────────────────────────────────────────────
    // API-Aufruf & Analyse
    // ─────────────────────────────────────────────

    using HttpClient http = new();

    var apiResponse = await http.GetAsync(generatedUrl);

    try
    {
        apiResponse.EnsureSuccessStatusCode();
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Fehler: {generatedUrl} - {ex.Message}");
        Console.WriteLine();
    }

    var json = await apiResponse.Content.ReadAsStringAsync();
    chatHistory.AddUserMessage($"Analysiere die folgenden JSON-Daten:\n{json}");

    var analysis = await chat.GetChatMessageContentAsync(chatHistory);

    Console.Write("Antwort: ");
    Console.WriteLine(analysis.Content);
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
