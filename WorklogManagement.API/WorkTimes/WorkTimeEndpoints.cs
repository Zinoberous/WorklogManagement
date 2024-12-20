using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models;
using WorklogManagement.Data.Context;

namespace WorklogManagement.API.WorkTimes;

internal static class WorkTimeEndpoints
{
    internal static IEndpointRouteBuilder RegisterWorkTimeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/worktimes").WithTags("WorkTimes");

        group.MapGet("", GetWorkTimesAsync);
        group.MapGet("/{id}", GetWorkTimeByIdAsync);
        group.MapGet("/dates", GetDatesWithWorkTimesAsync);
        group.MapPost("", SaveWorkTimeAsync);
        group.MapDelete("/{id}", DeleteWorkTimeAsync);

        return app;
    }

    private static async Task<Page<WorkTime>> GetWorkTimesAsync(WorklogManagementContext context, string sortBy = "Id", uint pageSize = 0, uint pageIndex = 0, string? filter = null)
    {
        var items = context.WorkTimes;

        var page = Page.GetQuery(items, out var totalItems, out var totalPages, ref pageIndex, pageSize, sortBy, filter, WorkTime.PropertyMappings);

        return new()
        {
            SortBy = sortBy,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Items = await page
                .Select(x => WorkTime.Map(x))
                .ToListAsync(),
        };
    }

    private static async Task<WorkTime> GetWorkTimeByIdAsync(WorklogManagementContext context, int id)
    {
        var item = await context.WorkTimes
            .SingleAsync(x => x.Id == id);

        return WorkTime.Map(item);
    }

    private static async Task<List<DateOnly>> GetDatesWithWorkTimesAsync(WorklogManagementContext context)
    {
        var dates = await context.WorkTimes
            .Select(x => x.Date)
            .Distinct()
            .ToListAsync();

        return dates;
    }

    private static async Task<WorkTime> SaveWorkTimeAsync(WorklogManagementContext context, WorkTime workTime)
    {
        await workTime.SaveAsync(context);

        return workTime;
    }

    private static async Task DeleteWorkTimeAsync(WorklogManagementContext context, int id)
    {
        await WorkTime.DeleteAsync(context, id);
    }
}
