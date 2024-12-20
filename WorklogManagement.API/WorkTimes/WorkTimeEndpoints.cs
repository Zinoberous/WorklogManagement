using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;

namespace WorklogManagement.API.WorkTimes;

internal static class WorkTimeEndpoints
{
    internal static IEndpointRouteBuilder RegisterWorkTimeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/worktimes").WithTags("WorkTimes");

        group.MapGet("", GetAsync);
        group.MapGet("/dates", GetDatesWithWorkTimesAsync);
        group.MapPost("", SaveAsync);
        group.MapDelete("/{id}", DeleteAsync);

        return app;
    }

    private static async Task<List<WorkTime>> GetAsync(WorklogManagementContext context, DateOnly from, DateOnly to)
    {
        return await context.WorkTimes
            .Where(x => x.Date >= from && x.Date <= to)
            .Select(x => WorkTime.Map(x))
            .ToListAsync();
    }

    private static async Task<List<DateOnly>> GetDatesWithWorkTimesAsync(WorklogManagementContext context)
    {
        var dates = await context.WorkTimes
            .Select(x => x.Date)
            .Distinct()
            .ToListAsync();

        return dates;
    }

    private static async Task<WorkTime> SaveAsync(WorklogManagementContext context, WorkTime workTime)
    {
        await workTime.SaveAsync(context);

        return workTime;
    }

    private static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        await WorkTime.DeleteAsync(context, id);
    }
}
