namespace WorklogManagement.API.Chat;

public record Answer
{
    public required Guid ChatId { get; init; }
    public required string Message { get; init; }
}
