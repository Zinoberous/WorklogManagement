using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace WorklogManagement.API.Chat;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddChatService(this IServiceCollection services, OpenAiOptions options)
    {
        var chatCompletionService = Kernel
            .CreateBuilder()
            .AddOpenAIChatCompletion(
                modelId: options.ModelId,
                apiKey: options.ApiKey,
                serviceId: options.ServiceId)
            .Build()
            .GetRequiredService<IChatCompletionService>();

        services.AddSingleton(chatCompletionService);

        services.AddSingleton<ISwaggerService, SwaggerService>();
        services.AddSingleton<IModelDescriptionService, ModelDescriptionService>();
        services.AddSingleton<IChatService, ChatService>();

        return services;
    }
}
