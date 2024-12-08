namespace WorklogManagement.Shared.Models;

public class Holiday
{
    public required DateOnly Date { get; init; }

    public required string Name { get; init; }
}
