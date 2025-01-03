namespace WorklogManagement.Shared.Models;

public record Ticket : IDataModel
{
    // TODO: uri self

    public int Id { get; init; }

    // TODO: uri ref ticket

    public int? RefId { get; init; }

    public required string Title { get; init; }

    public string? Description { get; init; }

    public required Enums.TicketStatus Status { get; init; }

    public string? StatusNote { get; init; }

    public DateTime? CreatedAt { get; init; }

    // TODO: uri ticket attachments

    public IEnumerable<TicketAttachment> Attachments { get; init; } = [];

    // TODO: uri ticket worklogs

    public TimeSpan TimeSpent { get; init; } = TimeSpan.Zero;
}
