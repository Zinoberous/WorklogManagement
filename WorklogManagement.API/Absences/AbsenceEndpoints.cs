using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models;
using WorklogManagement.Data.Context;

namespace WorklogManagement.API.Absences;

internal static class AbsenceEndpoints
{
    internal static IEndpointRouteBuilder RegisterAbsenceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/absences").WithTags("Absences");

        group.MapGet("", GetAbsencesAsync);
        group.MapGet("/{id}", GetAbsenceByIdAsync);
        group.MapGet("/dates", GetDatesWithAbsencesAsync);
        group.MapPost("", SaveAbsenceAsync);
        group.MapDelete("/{id}", DeleteAbsenceAsync);

        return app;
    }

    private static async Task<Page<Absence>> GetAbsencesAsync(WorklogManagementContext context, string sortBy = "Id", int pageSize = 0, int pageIndex = 0, string? filter = null)
    {
        var items = context.Absences;

        var page = Page.GetQuery(items, out var totalItems, out var totalPages, ref pageIndex, pageSize, sortBy, filter, Absence.PropertyMappings);

        return new()
        {
            SortBy = sortBy,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPages = totalPages,
            TotalItems = totalItems,
            Items = await page
                .Select(x => Absence.Map(x))
                .ToListAsync(),
        };
    }

    private static async Task<Absence> GetAbsenceByIdAsync(WorklogManagementContext context, int id)
    {
        var item = await context.Absences
            .SingleAsync(x => x.Id == id);

        return Absence.Map(item);
    }

    private static async Task<List<DateOnly>> GetDatesWithAbsencesAsync(WorklogManagementContext context)
    {
        var dates = await context.Absences
            .Select(x => x.Date)
            .Distinct()
            .ToListAsync();

        return dates;
    }

    private static async Task<Absence> SaveAbsenceAsync(WorklogManagementContext context, Absence absence)
    {
        await absence.SaveAsync(context);

        return absence;
    }

    private static async Task DeleteAbsenceAsync(WorklogManagementContext context, int id)
    {
        await Absence.DeleteAsync(context, id);
    }
}
