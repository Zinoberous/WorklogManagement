namespace WorklogManagement.Shared.Models;

public record TicketAttachment : Attachment
{
    public int TicketId { get; init; }
}
