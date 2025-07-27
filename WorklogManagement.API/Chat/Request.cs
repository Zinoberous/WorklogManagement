namespace WorklogManagement.API.Chat;

public record Request
{
    public Guid? ChatId { get; init; }
    public required string Prompt { get; init; }
}
