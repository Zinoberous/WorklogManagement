using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Services;

public interface IDataService
{
    Task<OvertimeInfo> GetOvertimeAsync();

    Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null);

    Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync();

    Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState);

    Task<List<WorkTime>> GetWorkTimesAsync(DateOnly date) => GetWorkTimesAsync(date, date);
    Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to);

    Task<List<DateOnly>> GetDatesWithWorkTimesAsync();

    Task<WorkTime> SaveWorkTimeAsync(WorkTime workTime);

    Task<List<Absence>> GetAbsencesAsync(DateOnly date) => GetAbsencesAsync(date, date);
    Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to);

    Task<List<DateOnly>> GetDatesWithAbsencesAsync();

    Task<Absence> SaveAbsenceAsync(Absence absence);

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

    private HttpClient CreateClient() => _httpClientFactory.CreateClient(nameof(WorklogManagement));

    public async Task<OvertimeInfo> GetOvertimeAsync()
    {
        using var client = CreateClient();

        var res = await client.GetAsync("statistics/overtime");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<OvertimeInfo>())!;
    }

    public async Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null)
    {
        using var client = CreateClient();

        var res = await client.GetAsync($"statistics/calendar{(year.HasValue ? $"?year={year.Value}" : string.Empty)}");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<Dictionary<CalendarEntryType, int>>())!;
    }

    public async Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync()
    {
        using var client = CreateClient();

        var res = await client.GetAsync("statistics/tickets");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<Dictionary<TicketStatus, int>>())!;
    }

    public async Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState)
    {
        using var client = CreateClient();

        var res = await client.GetAsync($"holidays/{federalState}?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<List<Holiday>>())!;
    }

    public async Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to)
    {
        using var client = CreateClient();

        var res = await client.GetAsync($"worktimes?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<List<WorkTime>>())!;
    }

    public async Task<List<DateOnly>> GetDatesWithWorkTimesAsync()
    {
        using var client = CreateClient();

        var res = await client.GetAsync("worktimes/dates");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<List<DateOnly>>())!;
    }

    public async Task<WorkTime> SaveWorkTimeAsync(WorkTime workTime)
    {
        using var client = CreateClient();

        var res = await client.PostAsJsonAsync("worktimes", workTime);

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<WorkTime>())!;
    }

    public async Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to)
    {
        using var client = CreateClient();

        var res = await client.GetAsync($"absences?from={from:yyyy-MM-dd} &to= {to:yyyy-MM-dd}");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<List<Absence>>())!;
    }

    public async Task<List<DateOnly>> GetDatesWithAbsencesAsync()
    {
        using var client = CreateClient();

        var res = await client.GetAsync("absences/dates");

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<List<DateOnly>>())!;
    }

    public async Task<Absence> SaveAbsenceAsync(Absence absence)
    {
        using var client = CreateClient();

        var res = await client.PostAsJsonAsync("absences", absence);

        res.EnsureSuccessStatusCode();

        return (await res.Content.ReadFromJsonAsync<Absence>())!;
    }
}