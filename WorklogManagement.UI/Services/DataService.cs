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

    Task DeleteWorkTimeAsync(int id);

    Task<List<Absence>> GetAbsencesAsync(DateOnly date) => GetAbsencesAsync(date, date);
    Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to);

    Task<List<DateOnly>> GetDatesWithAbsencesAsync();

    Task<Absence> SaveAbsenceAsync(Absence absence);

    Task DeleteAbsenceAsync(int id);

    Task<Page<Ticket>> GetTicketsPageByStatusFilterAsync(uint pageSize, uint pageIndex, IEnumerable<TicketStatus> statusFilter);

    Task<Page<Ticket>> GetTicketsPageBySearchAsync(uint pageSize, uint pageIndex, string search);

    Task<Ticket> SaveTicketAsync(Ticket ticket);

    Task DeleteTicketAsync(int id);
}

public class DataService(IHttpClientFactory httpClientFactory, IGlobalDataStateService dataStateService) : IDataService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IGlobalDataStateService _dataStateService = dataStateService;

    private HttpClient CreateClient() => _httpClientFactory.CreateClient(nameof(WorklogManagement));

    public async Task<OvertimeInfo> GetOvertimeAsync()
    {
        return await ExecuteWithDataStateAsync<OvertimeInfo>(async client => await client.GetAsync("statistics/overtime"));
    }

    public async Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null)
    {
        return await ExecuteWithDataStateAsync<Dictionary<CalendarEntryType, int>>(async client => await client.GetAsync($"statistics/calendar{(year.HasValue ? $"?year={year.Value}" : string.Empty)}"));
    }

    public async Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync()
    {
        return await ExecuteWithDataStateAsync<Dictionary<TicketStatus, int>>(async client => await client.GetAsync("statistics/tickets"));
    }

    public async Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState)
    {
        return await ExecuteWithDataStateAsync<List<Holiday>>(async client => await client.GetAsync($"holidays/{federalState}?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}"));
    }

    public async Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to)
    {
        var filter = Uri.EscapeDataString($@"Date >= ""{from:yyyy-MM-dd}"" && Date <= ""{to:yyyy-MM-dd}""");

        var page = await ExecuteWithDataStateAsync<Page<WorkTime>>(async client => await client.GetAsync($"worktimes?pageSize=0&filter={filter}"));

        return page.Items.ToList();
    }

    public async Task<List<DateOnly>> GetDatesWithWorkTimesAsync()
    {
        return await ExecuteWithDataStateAsync<List<DateOnly>>(async client => await client.GetAsync("worktimes/dates"));
    }

    public async Task<WorkTime> SaveWorkTimeAsync(WorkTime workTime)
    {
        return await ExecuteWithDataStateAsync<WorkTime>(async client => await client.PostAsJsonAsync("worktimes", workTime));
    }

    public async Task DeleteWorkTimeAsync(int id)
    {
        await ExecuteWithDataStateAsync(async client => await client.DeleteAsync($"worktimes/{id}"));
    }

    public async Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to)
    {
        var filter = Uri.EscapeDataString($@"Date >= ""{from:yyyy-MM-dd}"" && Date <= ""{to:yyyy-MM-dd}""");

        var page = await ExecuteWithDataStateAsync<Page<Absence>>(async client => await client.GetAsync($"absences?pageSize=0&filter={filter}"));

        return page.Items.ToList();
    }

    public async Task<List<DateOnly>> GetDatesWithAbsencesAsync()
    {
        return await ExecuteWithDataStateAsync<List<DateOnly>>(async client => await client.GetAsync("absences/dates"));
    }

    public async Task<Absence> SaveAbsenceAsync(Absence absence)
    {
        return await ExecuteWithDataStateAsync<Absence>(async client => await client.PostAsJsonAsync("absences", absence));
    }

    public async Task DeleteAbsenceAsync(int id)
    {
        await ExecuteWithDataStateAsync(async client => await client.DeleteAsync($"absences/{id}"));
    }

    public async Task<Page<Ticket>> GetTicketsPageByStatusFilterAsync(uint pageSize, uint pageIndex, IEnumerable<TicketStatus> statusFilter)
    {
        var filter = Uri.EscapeDataString($"status in ({string.Join(',', statusFilter.Select(x => (int)x))})");

        return await ExecuteWithDataStateAsync<Page<Ticket>>(async client => await client.GetAsync($"tickets?pageSize={pageSize}&pageIndex={pageIndex}&filter={filter}"));
    }

    public async Task<Page<Ticket>> GetTicketsPageBySearchAsync(uint pageSize, uint pageIndex, string search)
    {
        var filter = Uri.EscapeDataString($@"Title.Contains(""{search}"") || Description.Contains(""{search}"")");

        return await ExecuteWithDataStateAsync<Page<Ticket>>(async client => await client.GetAsync($"tickets?pageSize={pageSize}&pageIndex={pageIndex}&filter={filter}"));
    }

    public async Task<Ticket> SaveTicketAsync(Ticket ticket)
    {
        return await ExecuteWithDataStateAsync<Ticket>(async client => await client.PostAsJsonAsync("tickets", ticket));
    }

    public async Task DeleteTicketAsync(int id)
    {
        await ExecuteWithDataStateAsync(async client => await client.DeleteAsync($"tickets/{id}"));
    }

    private async Task<HttpResponseMessage> ExecuteWithDataStateAsync(Func<HttpClient, Task<HttpResponseMessage>> action)
        => await ExecuteWithDataStateAsync<HttpResponseMessage>(action);

    private async Task<T> ExecuteWithDataStateAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> action)
    {
        using var client = CreateClient();

        _dataStateService.StartOperation();

        try
        {
            var res = await action(client);

            res.EnsureSuccessStatusCode();

            if (typeof(T) == typeof(HttpResponseMessage))
            {
                return (T)(object)res;
            }

            return (await res.Content.ReadFromJsonAsync<T>())!;
        }
        catch
        {
            _dataStateService.SetError();
            throw;
        }
        finally
        {
            _dataStateService.EndOperation();
        }
    }
}
