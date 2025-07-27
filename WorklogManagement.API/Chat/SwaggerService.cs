using System.Text.Json;

namespace WorklogManagement.API.Chat;

public interface ISwaggerService
{
    Task<IEnumerable<string>> GetGetRoutesAsync(string swaggerUrl, CancellationToken cancellationToken = default);
}

public class SwaggerService(IHttpClientFactory httpClientFactory) : ISwaggerService
{
    public async Task<IEnumerable<string>> GetGetRoutesAsync(string swaggerUrl, CancellationToken cancellationToken = default)
    {
        using var http = httpClientFactory.CreateClient();
        using var stream = await http.GetStreamAsync(swaggerUrl, cancellationToken);

        var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
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
