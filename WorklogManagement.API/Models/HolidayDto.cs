namespace WorklogManagement.API.Models;

public class HolidayDto
{
    public required DateOnly Date { get; init; }

    public required string LocalName { get; init; }

    public IEnumerable<string>? Counties { get; init; }
}
