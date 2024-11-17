namespace WorklogManagement.Service.Models;

public abstract class Attachment
{
    protected int? _id;
    public int? Id { get => _id; init => _id = value; }

    public required string Name { get; init; }

    public required string Data { get; init; }

    public string? Comment { get; init; }
}
