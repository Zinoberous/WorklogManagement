namespace WorklogManagement.Shared.Models;

public record OvertimeInfo
{
    public TimeSpan Total { get; init; } = TimeSpan.Zero;

    public TimeSpan Office { get; init; } = TimeSpan.Zero;

    public TimeSpan Mobile { get; init; } = TimeSpan.Zero;
}
