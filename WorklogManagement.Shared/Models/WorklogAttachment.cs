namespace WorklogManagement.Shared.Models;

public record WorklogAttachment : Attachment
{
    // TODO: uri worklog

    public required int WorklogId { get; init; }
}
