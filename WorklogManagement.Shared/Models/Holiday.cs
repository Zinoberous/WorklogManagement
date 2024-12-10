namespace WorklogManagement.Shared.Models;

public record Holiday
{
    public required DateOnly Date { get; init; }

    public required string Name { get; init; }
}
