namespace WorklogManagement.Shared.Models;

public record Ticket : IDataModel
{
    public int? Id { get; init; }

    public int? RefId { get; init; }

    public required string Title { get; init; }

    public string? Description { get; init; }

    public required Enums.TicketStatus Status { get; init; }

    public string? StatusNote { get; init; }

    public DateTime? CreatedAt { get; init; }

    public required int TimeSpentMinutes { get; init; }

    public required int AttachmentsCount { get; init; }
}
