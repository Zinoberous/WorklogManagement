namespace WorklogManagement.Shared.Models;

public record Ticket : IDataModel
{
    // TODO: uri self

    public int Id { get; init; }

    // TODO: uri ref ticket

    public RefTicket? Ref { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public Enums.TicketStatus Status { get; set; } = Enums.TicketStatus.Todo;

    public DateTime StatusStartedAt { get; init; } = DateTime.UtcNow;

    public string? StatusNote { get; set; }

    public DateTime? CreatedAt { get; init; }

    // TODO: uri ticket attachments

    public IEnumerable<TicketAttachment> Attachments { get; init; } = [];

    // TODO: uri ticket worklogs

    public TimeSpan TimeSpent { get; init; } = TimeSpan.Zero;
}
