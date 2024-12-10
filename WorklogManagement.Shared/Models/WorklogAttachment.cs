namespace WorklogManagement.Shared.Models;

public record WorklogAttachment : Attachment
{
    public required int WorklogId { get; init; }
}
