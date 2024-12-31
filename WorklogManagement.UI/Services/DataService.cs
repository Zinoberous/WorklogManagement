using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Services;

public interface IDataService
{
    Task<OvertimeInfo> GetOvertimeAsync();

    Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null);

    Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync();

    Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState);

    Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to);

    Task<List<DateOnly>> GetDatesWithWorkTimesAsync();

    Task<WorkTime> SaveWorkTimeAsync(WorkTime workTime);

    Task DeleteWorkTimeAsync(int id);

    Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to);

    Task<List<DateOnly>> GetDatesWithAbsencesAsync();

    Task<Absence> SaveAbsenceAsync(Absence absence);

    Task DeleteAbsenceAsync(int id);

    Task<Page<Ticket>> GetTicketsPageByStatusFilterAsync(uint pageSize, uint pageIndex, IEnumerable<TicketStatus> statusFilter);

    Task<Page<Ticket>> GetTicketsPageBySearchAsync(uint pageSize, uint pageIndex, string search);

    Task<Ticket> SaveTicketAsync(Ticket ticket);

    Task DeleteTicketAsync(int id);
}

public class DataService(ILoggerService<DataService> logger, IHttpClientFactory httpClientFactory, IGlobalDataStateService dataStateService) : IDataService
{
    private readonly ILoggerService<DataService> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IGlobalDataStateService _dataStateService = dataStateService;

    private HttpClient CreateClient() => _httpClientFactory.CreateClient(nameof(WorklogManagement));

    public async Task<OvertimeInfo> GetOvertimeAsync()
    {
        return await ExecuteWithDataStateAsync<OvertimeInfo>(HttpMethod.Get, "statistics/overtime");
    }

    public async Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null)
    {
        return await ExecuteWithDataStateAsync<Dictionary<CalendarEntryType, int>>(HttpMethod.Get, $"statistics/calendar{(year.HasValue ? $"?year={year.Value}" : string.Empty)}");
    }

    public async Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync()
    {
        return await ExecuteWithDataStateAsync<Dictionary<TicketStatus, int>>(HttpMethod.Get, "statistics/tickets");
    }

    public async Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState)
    {
        return await ExecuteWithDataStateAsync<List<Holiday>>(HttpMethod.Get, $"holidays/{federalState}?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
    }

    public async Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to)
    {
        var filter = Uri.EscapeDataString($@"Date >= ""{from:yyyy-MM-dd}"" && Date <= ""{to:yyyy-MM-dd}""");

        var page = await ExecuteWithDataStateAsync<Page<WorkTime>>(HttpMethod.Get, $"worktimes?pageSize=0&filter={filter}");

        return page.Items.ToList();
    }

    public async Task<List<DateOnly>> GetDatesWithWorkTimesAsync()
    {
        return await ExecuteWithDataStateAsync<List<DateOnly>>(HttpMethod.Get, "worktimes/dates");
    }

    public async Task<WorkTime> SaveWorkTimeAsync(WorkTime workTime)
    {
        return await ExecuteWithDataStateAsync<WorkTime>(HttpMethod.Post, "worktimes", workTime);
    }

    public async Task DeleteWorkTimeAsync(int id)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"worktimes/{id}");
    }

    public async Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to)
    {
        var filter = Uri.EscapeDataString($@"Date >= ""{from:yyyy-MM-dd}"" && Date <= ""{to:yyyy-MM-dd}""");

        var page = await ExecuteWithDataStateAsync<Page<Absence>>(HttpMethod.Get, $"absences?pageSize=0&filter={filter}");

        return page.Items.ToList();
    }

    public async Task<List<DateOnly>> GetDatesWithAbsencesAsync()
    {
        return await ExecuteWithDataStateAsync<List<DateOnly>>(HttpMethod.Get, "absences/dates");
    }

    public async Task<Absence> SaveAbsenceAsync(Absence absence)
    {
        return await ExecuteWithDataStateAsync<Absence>(HttpMethod.Post, "absences", absence);
    }

    public async Task DeleteAbsenceAsync(int id)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"absences/{id}");
    }

    public async Task<Page<Ticket>> GetTicketsPageByStatusFilterAsync(uint pageSize, uint pageIndex, IEnumerable<TicketStatus> statusFilter)
    {
        var filter = Uri.EscapeDataString($"status in ({string.Join(',', statusFilter.Select(x => (int)x))})");

        return await ExecuteWithDataStateAsync<Page<Ticket>>(HttpMethod.Get, $"tickets?pageSize={pageSize}&pageIndex={pageIndex}&filter={filter}");
    }

    public async Task<Page<Ticket>> GetTicketsPageBySearchAsync(uint pageSize, uint pageIndex, string search)
    {
        var filter = Uri.EscapeDataString($@"Title.Contains(""{search}"") || Description.Contains(""{search}"")");

        return await ExecuteWithDataStateAsync<Page<Ticket>>(HttpMethod.Get, $"tickets?pageSize={pageSize}&pageIndex={pageIndex}&filter={filter}");
    }

    public async Task<Ticket> SaveTicketAsync(Ticket ticket)
    {
        return await ExecuteWithDataStateAsync<Ticket>(HttpMethod.Post, "tickets", ticket);
    }

    public async Task DeleteTicketAsync(int id)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"tickets/{id}");
    }

    private async Task<HttpResponseMessage> ExecuteWithDataStateAsync(HttpMethod method, string requestUri, object? content = null)
        => await ExecuteWithDataStateAsync<HttpResponseMessage>(method, requestUri, content);

    private async Task<T> ExecuteWithDataStateAsync<T>(HttpMethod method, string requestUri, object? content = null)
    {
        _dataStateService.StartOperation();

        using var client = CreateClient();

        var api = $"{client.BaseAddress}{requestUri}";

        if (content is null)
        {
            _logger.LogInformation("{Method}: {API}", method, api);
        }
        else
        {
            _logger.LogInformation("{Method}: {API} | {@Content}", method, api, content);
        }

        try
        {
            var jsonContent = JsonContent.Create(content);

            var res = await client.SendAsync(new(method, requestUri) { Content = jsonContent });
            
            res.EnsureSuccessStatusCode();

            if (typeof(T) == typeof(HttpResponseMessage))
            {
                return (T)(object)res;
            }

            return (await res.Content.ReadFromJsonAsync<T>())!;
        }
        catch (Exception ex)
        {
            _dataStateService.SetError();
            _logger.LogError(ex, "Fehler beim {Method}-Aufruf von {API}", method, api);
            throw;
        }
        finally
        {
            _dataStateService.EndOperation();
        }
    }
}
