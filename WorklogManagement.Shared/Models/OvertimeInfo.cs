﻿namespace WorklogManagement.Shared.Models;

public class OvertimeInfo
{
    public required int TotalMinutes { get; init; }

    public required int OfficeMinutes { get; init; }

    public required int MobileMinutes { get; init; }
}