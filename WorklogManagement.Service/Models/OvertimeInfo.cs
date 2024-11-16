namespace WorklogManagement.Service.Models;

public class OvertimeInfo
{
    public required int TotalSeconds { get; init; }
    public required int OfficeSeconds { get; init; }
    public required int MobileSeconds { get; init; }
}
