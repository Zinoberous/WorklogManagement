namespace WorklogManagement.Shared.Models;

public record TicketAttachment : Attachment
{
    // TODO: uri ticket

    public int TicketId { get; init; }
}
