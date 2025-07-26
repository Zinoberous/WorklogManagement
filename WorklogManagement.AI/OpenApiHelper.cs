using System.Text.Json;

namespace WorklogManagement.AI;

public static class OpenApiHelper
{
    public static async Task<List<string>> GetGetRoutesAsync(string swaggerUrl)
    {
        using var http = new HttpClient();
        using var stream = await http.GetStreamAsync(swaggerUrl);

        var document = await JsonDocument.ParseAsync(stream);
        var root = document.RootElement;

        var getRoutes = new List<string>();

        if (root.TryGetProperty("paths", out var pathsElement))
        {
            foreach (var path in pathsElement.EnumerateObject())
            {
                var methods = path.Value;

                if (methods.TryGetProperty("get", out _))
                {
                    getRoutes.Add(path.Name);
                }
            }
        }

        return getRoutes;
    }
}
