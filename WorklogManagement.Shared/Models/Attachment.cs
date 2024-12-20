namespace WorklogManagement.Shared.Models;

public record Attachment : IDataModel
{
    // TODO: uri self

    public int Id { get; init; }

    public required string Name { get; init; }

    public required string Data { get; init; }

    public string? Comment { get; init; }
}
