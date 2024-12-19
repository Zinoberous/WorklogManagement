using WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Holidays;

internal static class HolidayEndpoints
{
    internal static IEndpointRouteBuilder RegisterHolidayEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/holidays").WithTags("Holidays");

        group.MapGet("/{federalState}", Get);

        return app;
    }

    private static async Task<List<Holiday>> Get(IHttpClientFactory httpClientFactory, string federalState, DateOnly from, DateOnly to)
    {
        using var client = httpClientFactory.CreateClient();

        List<Holiday> holidays = [];

        for (var year = from.Year; year <= to.Year; year++)
        {
            var res = await client.GetAsync($"https://date.nager.at/api/v3/PublicHolidays/{year}/DE");

            res.EnsureSuccessStatusCode();

            var yearHolidays = await res.Content.ReadFromJsonAsync<IEnumerable<HolidayDto>>();

            if (yearHolidays != null)
            {
                var relevantHolidays = yearHolidays
                    .Where(h => h.Date >= from && h.Date <= to && (h.Counties == null || h.Counties.Contains(federalState)))
                    .Select(x => new Holiday { Date = x.Date, Name = x.LocalName });

                holidays.AddRange(relevantHolidays);
            }

            holidays.AddRange([
                new() { Date = new(year, 12, 24), Name = "Heiligabend" },
                new() { Date = new(year, 12, 31), Name = "Silvester" },
            ]);
        }

        return holidays;
    }
}
