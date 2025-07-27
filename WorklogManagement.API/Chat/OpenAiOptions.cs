namespace WorklogManagement.API.Chat;

public record OpenAiOptions
{
    public required string ApiKey { get; init; }
    public required string ModelId { get; init; }
    public required string ServiceId { get; init; }
}
