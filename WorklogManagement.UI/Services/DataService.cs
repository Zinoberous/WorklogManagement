using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.Services;

public interface IDataService
{
    Task<OvertimeInfo> GetOvertimeAsync();

    Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null);

    Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync();

    Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState);

    Task<List<WorkTime>> GetWorkTimesAsync(DateOnly date) => GetWorkTimesAsync(date, date);
    Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to);

    Task<List<Absence>> GetAbsencesAsync(DateOnly date) => GetAbsencesAsync(date, date);
    Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to);

    // TODO: Tickets

    // TODO: TicketAttachments

    // TODO: TicketsStatusLogs

    // TODO: Worklogs

    // TODO: WorklogAttachments

    //Task<TDataModel> SaveAsync<TDataModel>(TDataModel item) where TDataModel : IDataModel;

    //Task DeleteAsync<TDataModel>(int id) where TDataModel : IDataModel;
}

public class DataService(IHttpClientFactory httpClientFactory) : IDataService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<OvertimeInfo> GetOvertimeAsync()
    {
        using var client = _httpClientFactory.CreateClient();

        var res = await client.GetAsync($"https://localhost:7182/statistics/overtime");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<OvertimeInfo>())!;
    }

    public async Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null)
    {
        using var client = _httpClientFactory.CreateClient();

        var res = await client.GetAsync($"https://localhost:7182/statistics/calendar{(year.HasValue ? $"?year={year.Value}" : string.Empty)}");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<Dictionary<CalendarEntryType, int>>())!;
    }

    public async Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync()
    {
        using var client = _httpClientFactory.CreateClient();

        var res = await client.GetAsync($"https://localhost:7182/statistics/tickets");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<Dictionary<TicketStatus, int>>())!;
    }

    public async Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState)
    {
        using var client = _httpClientFactory.CreateClient();

        List<Holiday> holidays = new();

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
        }

        return holidays;
    }

    public async Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to)
    {
        using var client = _httpClientFactory.CreateClient();

        var res = await client.GetAsync($"https://localhost:7182/worktimes?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<List<WorkTime>>())!;
    }

    public async Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to)
    {
        using var client = _httpClientFactory.CreateClient();

        var res = await client.GetAsync($"https://localhost:7182/absences?from={from:yyyy-MM-dd} &to= {to:yyyy-MM-dd}");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<List<Absence>>())!;
    }

    //public async Task<TDataModel> SaveAsync<TDataModel>(TDataModel item)
    //    where TDataModel : IDataModel
    //{
    //    await ExecuteAsync(context =>
    //        (Task)typeof(TDataModel)
    //            .GetMethod("SaveAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
    //            .Invoke(item, [context])!
    //    );

    //    return item;
    //}

    //public async Task DeleteAsync<TDataModel>(int id)
    //    where TDataModel : IDataModel
    //{
    //    await ExecuteAsync(context =>
    //        (Task)typeof(TDataModel)
    //            .GetMethod("DeleteAsync", BindingFlags.Static | BindingFlags.NonPublic)!
    //            .Invoke(null, [context, id])!
    //    );
    //}
}
