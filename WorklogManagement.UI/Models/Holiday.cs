namespace WorklogManagement.UI.Models;

/// <summary>
/// Holidays, die in der UI angezeigt werden.
/// </summary>
public class Holiday
{
    public required DateOnly Date { get; set; }

    public required string Name { get; set; }
}

/// <summary>
/// Representiert Holiday, wie es von der API als JSON geliefert wird, mit der Filtermöglichkeit auf Counties.
/// </summary>
public class HolidayDto
{
    public required DateOnly Date { get; set; }

    public required string LocalName { get; set; }

    public IEnumerable<string>? Counties { get; set; }
}
