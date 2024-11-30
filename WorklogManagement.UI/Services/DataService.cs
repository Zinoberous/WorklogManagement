using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.Services;

public interface IDataService
{
    Task<OvertimeInfo> GetOvertimeAsync();

    Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null);

    Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync();

    Task<List<Holiday>> GetHolidaysAsync(int year, string federalState);

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

public class DataService(IHttpClientFactory httpClientFactory, INotifier notifier) : IDataService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly INotifier _notifier = notifier;

    public async Task<OvertimeInfo> GetOvertimeAsync()
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();

            var res = await client.GetAsync($"https://localhost:7182/statistics/overtime");

            res.EnsureSuccessStatusCode();

            return (await res.Content.ReadFromJsonAsync<OvertimeInfo>())!;
        }
        catch (Exception ex)
        {
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Überstunden!", ex);

            throw;
        }
    }

    public async Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();

            var res = await client.GetAsync($"https://localhost:7182/statistics/calendar{(year.HasValue ? $"?year={year.Value}" : string.Empty)}");

            res.EnsureSuccessStatusCode();

            return (await res.Content.ReadFromJsonAsync<Dictionary<CalendarEntryType, int>>())!;
        }
        catch (Exception ex)
        {
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Kalendereinträge!", ex);

            throw;
        }
    }

    public async Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync()
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();

            var res = await client.GetAsync($"https://localhost:7182/statistics/tickets");

            res.EnsureSuccessStatusCode();

            return (await res.Content.ReadFromJsonAsync<Dictionary<TicketStatus, int>>())!;
        }
        catch (Exception ex)
        {
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Tickets!", ex);

            throw;
        }
    }

    public async Task<List<Holiday>> GetHolidaysAsync(int year, string federalState)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();

            var res = await client.GetAsync($"https://date.nager.at/api/v3/PublicHolidays/{year}/DE");

            res.EnsureSuccessStatusCode();

            var holidays = await res.Content.ReadFromJsonAsync<IEnumerable<HolidayDto>>();

            return holidays?
                .Where(h => h.Counties == null || h.Counties.Contains(federalState))
                .Select(x => new Holiday { Date = x.Date, Name = x.LocalName })
                .ToList() ?? [];
        }
        catch (Exception ex)
        {
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Feiertage!", ex);

            throw;
        }
    }

    public async Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();

            var res = await client.GetAsync($"https://localhost:7182/worktimes?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");

            res.EnsureSuccessStatusCode();

            return (await res.Content.ReadFromJsonAsync<List<WorkTime>>())!;
        }
        catch (Exception ex)
        {
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Arbeitszeiten!", ex);

            throw;
        }
    }

    public async Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();

            var res = await client.GetAsync($"https://localhost:7182/absences?from={from:yyyy-MM-dd} &to= {to:yyyy-MM-dd}");

            res.EnsureSuccessStatusCode();

            return (await res.Content.ReadFromJsonAsync<List<Absence>>())!;
        }
        catch (Exception ex)
        {
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Abwesenheiten!", ex);

            throw;
        }
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
