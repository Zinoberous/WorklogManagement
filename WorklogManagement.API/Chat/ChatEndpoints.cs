using Microsoft.AspNetCore.Mvc;

namespace WorklogManagement.API.Chat;

internal static class ChatEndpoints
{
    internal static IEndpointRouteBuilder RegisterChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/chat").WithTags("Chat");

        group.MapPost("", PostAsync);

        return app;
    }

    private static async Task<Answer> PostAsync(
        HttpContext httpContext,
        [FromBody] Request request,
        IChatService chatService)
    {
        var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        return await chatService.AskAsync(request, baseUrl);
    }
}
