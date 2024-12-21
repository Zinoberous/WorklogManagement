namespace WorklogManagement.Shared.Models;

public record Page<TData>
    where TData : IDataModel
{
    // TODO: uri first page

    // TODO: uri last page

    // TODO: uri? next page

    // TODO: uri? previous page

    public required string SortBy { get; init; }

    public required uint PageSize { get; init; }

    public required uint PageIndex { get; init; }

    // TODO: filter

    public required uint TotalPages { get; init; }

    public required uint TotalItems { get; init; }

    public required IEnumerable<TData> Items { get; init; }

    public static Page<TData> Empty => new()
    {
        SortBy = string.Empty,
        PageSize = 0,
        PageIndex = 0,
        TotalPages = 0,
        TotalItems = 0,
        Items = []
    };
}
