using Microsoft.AspNetCore.Mvc;
using WorklogManagement.API.Models;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HolidaysController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    [HttpGet("{federalState}")]
    public async Task<List<Holiday>> Get(string federalState, DateOnly from, DateOnly to)
    {
        using var client = _httpClientFactory.CreateClient();

        List<Holiday> holidays = [];

        for (var year = from.Year; year <= to.Year; year++)
        {
            var res = await client.GetAsync($"https://date.nager.at/api/v3/PublicHolidays/{year}/DE");

            res.EnsureSuccessStatusCode();

            var yearHolidays = await res.Content.ReadFromJsonAsync<IEnumerable<HolidayDto>>();

            if (yearHolidays != null)
            {
                holidays.AddRange(yearHolidays
                    .Where(h => h.Date >= from && h.Date <= to && (h.Counties == null || h.Counties.Contains(federalState)))
                    .Select(x => new Holiday { Date = x.Date, Name = x.LocalName }));
            }

            holidays.AddRange(
            [
                new() { Date = new(year, 12, 24), Name = "Heiligabend" },
                new() { Date = new(year, 12, 31), Name = "Silvester" },
            ]);
        }

        return holidays;
    }
}
