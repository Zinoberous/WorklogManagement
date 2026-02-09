namespace WorklogManagement.Shared.Models;

public record Worklog : IDataModel
{
    // TODO: uri self

    public int Id { get; init; }

    // TODO: uri worklogs of day

    public required DateOnly Date { get; init; }

    // TODO: uri ticket

    public required RefTicket Ticket { get; init; }

    public string? Description { get; init; }

    public required TimeSpan TimeSpent { get; init; }

    // TODO: uri worklog attachments

    public IEnumerable<WorklogAttachment> Attachments { get; init; } = [];
}
