namespace WorklogManagement.Shared.Models;

public record Page<TDataModel>
    where TDataModel : IDataModel
{
    public required int PageSize { get; init; }

    public required int PageIndex { get; init; }

    public required int TotalPages { get; init; }

    public required int TotalItems { get; init; }

    public required IEnumerable<TDataModel> Items { get; init; }
}
