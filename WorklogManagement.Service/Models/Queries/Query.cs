namespace WorklogManagement.Service.Models.Queries;

public abstract class Query
{
    public int PageSize { get; init; } = 0;

    public int PageIndex { get; init; } = 0;
}
