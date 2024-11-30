namespace WorklogManagement.Shared.Models;

public class WorklogAttachment : Attachment
{
    public required int WorklogId { get; init; }
}
