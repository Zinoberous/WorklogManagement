using System.Text.Json;

namespace WorklogManagement.API.Helper
{
    internal static class RequestHelper
    {
        internal static async Task<T?> GetBodyAsync<T>(HttpRequest request)
        {
            using StreamReader stream = new(request.Body);

            var body = await stream.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(body);
        }
    }
}
