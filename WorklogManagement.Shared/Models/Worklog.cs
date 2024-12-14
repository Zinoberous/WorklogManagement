namespace WorklogManagement.Shared.Models;

public record Worklog : IDataModel
{
    public int? Id { get; init; }

    public required DateOnly Date { get; init; }

    public required int TicketId { get; init; }

    public required string TicketTitle { get; init; }

    public string? Description { get; init; }

    public required TimeSpan TimeSpent { get; init; }

    public int AttachmentsCount { get; init; }
}
