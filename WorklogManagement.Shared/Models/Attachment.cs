namespace WorklogManagement.Shared.Models;

public class Attachment : IDataModel
{
    public int? Id { get; init; }

    public required string Name { get; init; }

    public required string Data { get; init; }

    public string? Comment { get; init; }
}
