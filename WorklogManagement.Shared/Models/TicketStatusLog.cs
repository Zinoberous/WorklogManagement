namespace WorklogManagement.Shared.Models;

public record TicketStatusLog : IDataModel
{
    // TODO: uri self

    public int Id { get; init; }

    // TODO: uri ticket

    public required int TicketId { get; init; }

    public required string TicketTitle { get; init; }

    public required Enums.TicketStatus Status { get; init; }

    public required DateTime StartedAt { get; init; }

    public string? Note { get; init; }
}
