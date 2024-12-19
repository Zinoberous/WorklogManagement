using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;

namespace WorklogManagement.API.Absences;

internal static class AbsenceEndpoints
{
    internal static IEndpointRouteBuilder RegisterAbsenceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/absences").WithTags("Absences");

        group.MapGet("", Get);
        group.MapGet("/dates", GetDatesWithAbsences);
        group.MapPost("", Save);
        group.MapDelete("/{id}", Delete);

        return app;
    }

    private static async Task<List<Absence>> Get(WorklogManagementContext context, DateOnly from, DateOnly to)
    {
        return await context.Absences
            .Where(x => x.Date >= from && x.Date <= to)
            .Select(x => Absence.Map(x))
            .ToListAsync();
    }

    private static async Task<List<DateOnly>> GetDatesWithAbsences(WorklogManagementContext context)
    {
        var dates = await context.Absences
            .Select(x => x.Date)
            .Distinct()
            .ToListAsync();

        return dates;
    }

    private static async Task<Absence> Save(WorklogManagementContext context, Absence absence)
    {
        await absence.SaveAsync(context);

        return absence;
    }

    private static async Task Delete(WorklogManagementContext context, int id)
    {
        await Absence.DeleteAsync(context, id);
    }
}
