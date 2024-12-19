namespace WorklogManagement.API.Holidays;

public record HolidayDto
{
    public required DateOnly Date { get; init; }

    public required string LocalName { get; init; }

    public IEnumerable<string>? Counties { get; init; }
}
