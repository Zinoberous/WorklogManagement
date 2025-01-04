namespace WorklogManagement.Shared.Models;

public record RefTicket
{
    public required int Id { get; init; }
    public required string Title { get; init; }
}
