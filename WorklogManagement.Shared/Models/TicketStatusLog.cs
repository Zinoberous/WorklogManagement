namespace WorklogManagement.Shared.Models;

public record TicketStatusLog : IDataModel
{
    public int? Id { get; init; }

    public required int TicketId { get; init; }

    public required string TicketTitle { get; init; }

    public required Enums.TicketStatus Status { get; init; }

    public required DateTime StartedAt { get; init; }

    public string? Note { get; init; }
}
