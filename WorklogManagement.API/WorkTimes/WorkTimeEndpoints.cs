using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;

namespace WorklogManagement.API.WorkTimes;

internal static class WorkTimeEndpoints
{
    internal static IEndpointRouteBuilder RegisterWorkTimeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/worktimes").WithTags("WorkTimes");

        group.MapGet("", Get);
        group.MapGet("/dates", GetDatesWithWorkTimes);
        group.MapPost("", Save);
        group.MapDelete("/{id}", Delete);

        return app;
    }

    [HttpGet]
    private static async Task<List<WorkTime>> Get(WorklogManagementContext context, DateOnly from, DateOnly to)
    {
        return await context.WorkTimes
            .Where(x => x.Date >= from && x.Date <= to)
            .Select(x => WorkTime.Map(x))
            .ToListAsync();
    }

    [HttpGet("dates")]
    private static async Task<List<DateOnly>> GetDatesWithWorkTimes(WorklogManagementContext context)
    {
        var dates = await context.WorkTimes
            .Select(x => x.Date)
            .Distinct()
            .ToListAsync();

        return dates;
    }

    [HttpPost]
    private static async Task<WorkTime> Save(WorklogManagementContext context, WorkTime workTime)
    {
        await workTime.SaveAsync(context);

        return workTime;
    }

    [HttpDelete("{id}")]
    private static async Task Delete(WorklogManagementContext context, int id)
    {
        await WorkTime.DeleteAsync(context, id);
    }
}
