using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Services;

public interface IDataService
{
    #region statistics

    Task<OvertimeInfo> GetOvertimeAsync();

    Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null);

    Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync();

    #endregion

    #region calendar

    Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState);

    Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to);

    Task<List<DateOnly>> GetDatesWithWorkTimesAsync();

    Task<WorkTime> SaveWorkTimeAsync(WorkTime workTime);

    Task DeleteWorkTimeAsync(int id);

    Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to);

    Task<List<DateOnly>> GetDatesWithAbsencesAsync();

    Task<Absence> SaveAbsenceAsync(Absence absence);

    Task DeleteAbsenceAsync(int id);

    #endregion

    #region ticket

    Task<Ticket> GetTicketAsync(int id);

    Task<Page<Ticket>> GetTicketsAsync(int pageSize, int pageIndex, string? filter = null);

    Task<Ticket> SaveTicketAsync(Ticket ticket);

    Task DeleteTicketAsync(int id);

    Task<List<TicketStatusLog>> GetTicketStatusLogsAsync(int ticketId);

    Task<TicketStatusLog> SaveTicketStatusLogAsync(TicketStatusLog ticketStatusLog);

    #endregion

    #region worklog

    Task<Worklog> GetWorklogAsync(int id);

    Task<Page<Worklog>> GetWorklogsAsync(int pageSize, int pageIndex, string? filter = null);

    Task<Worklog> SaveWorklogAsync(Worklog worklog);

    Task DeleteWorklogAsync(int id);

    #endregion
}

public class DataService(ILoggerService<DataService> logger, IHttpClientFactory httpClientFactory, IGlobalDataStateService dataStateService) : IDataService
{
    private readonly ILoggerService<DataService> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IGlobalDataStateService _dataStateService = dataStateService;

    private HttpClient CreateClient() => _httpClientFactory.CreateClient(nameof(WorklogManagement));

    #region statistics

    public async Task<OvertimeInfo> GetOvertimeAsync()
    {
        return await ExecuteWithDataStateAsync<OvertimeInfo>(HttpMethod.Get, "statistics/overtime");
    }

    public async Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null)
    {
        var queryParams = year.HasValue ? new Dictionary<string, string> { ["year"] = year.Value.ToString() } : null;

        return await ExecuteWithDataStateAsync<Dictionary<CalendarEntryType, int>>(HttpMethod.Get, "statistics/calendar", queryParams);
    }

    public async Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync()
    {
        return await ExecuteWithDataStateAsync<Dictionary<TicketStatus, int>>(HttpMethod.Get, "statistics/tickets");
    }

    #endregion

    #region calendar

    public async Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "from", from.ToString("yyyy-MM-dd") },
            { "to", to.ToString("yyyy-MM-dd") }
        };

        return await ExecuteWithDataStateAsync<List<Holiday>>(HttpMethod.Get, $"holidays/{federalState}", queryParams);
    }

    public async Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "pageSize", "0" },
            { "filter", $@"Date >= ""{from:yyyy-MM-dd}"" && Date <= ""{to:yyyy-MM-dd}""" }
        };

        var page = await ExecuteWithDataStateAsync<Page<WorkTime>>(HttpMethod.Get, "worktimes", queryParams);

        return page.Items.ToList();
    }

    public async Task<List<DateOnly>> GetDatesWithWorkTimesAsync()
    {
        return await ExecuteWithDataStateAsync<List<DateOnly>>(HttpMethod.Get, "worktimes/dates");
    }

    public async Task<WorkTime> SaveWorkTimeAsync(WorkTime workTime)
    {
        return await ExecuteWithDataStateAsync<WorkTime>(HttpMethod.Post, "worktimes", content: workTime);
    }

    public async Task DeleteWorkTimeAsync(int id)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"worktimes/{id}");
    }

    public async Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "pageSize", "0" },
            { "filter", $@"Date >= ""{from:yyyy-MM-dd}"" && Date <= ""{to:yyyy-MM-dd}""" }
        };

        var page = await ExecuteWithDataStateAsync<Page<Absence>>(HttpMethod.Get, "absences", queryParams);

        return page.Items.ToList();
    }

    public async Task<List<DateOnly>> GetDatesWithAbsencesAsync()
    {
        return await ExecuteWithDataStateAsync<List<DateOnly>>(HttpMethod.Get, "absences/dates");
    }

    public async Task<Absence> SaveAbsenceAsync(Absence absence)
    {
        return await ExecuteWithDataStateAsync<Absence>(HttpMethod.Post, "absences", content: absence);
    }

    public async Task DeleteAbsenceAsync(int id)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"absences/{id}");
    }

    #endregion

    #region ticket

    public async Task<Ticket> GetTicketAsync(int id)
    {
        return await ExecuteWithDataStateAsync<Ticket>(HttpMethod.Get, $"tickets/{id}");
    }

    public async Task<Page<Ticket>> GetTicketsAsync(int pageSize, int pageIndex, string? filter = null)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "pageSize", pageSize.ToString() },
            { "pageIndex", pageIndex.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(filter))
        {
            queryParams.Add("filter", filter);
        }

        return await ExecuteWithDataStateAsync<Page<Ticket>>(HttpMethod.Get, "tickets", queryParams);
    }

    public async Task<Ticket> SaveTicketAsync(Ticket ticket)
    {
        return await ExecuteWithDataStateAsync<Ticket>(HttpMethod.Post, "tickets", content: ticket);
    }

    public async Task DeleteTicketAsync(int id)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"tickets/{id}");
    }

    public async Task<List<TicketStatusLog>> GetTicketStatusLogsAsync(int ticketId)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "ticketId", ticketId.ToString() }
        };

        var page = await ExecuteWithDataStateAsync<Page<TicketStatusLog>>(HttpMethod.Get, "ticketStatusLogs", queryParams);

        return page.Items.ToList();
    }

    public async Task<TicketStatusLog> SaveTicketStatusLogAsync(TicketStatusLog ticketStatusLog)
    {
        return await ExecuteWithDataStateAsync<TicketStatusLog>(HttpMethod.Post, "ticketStatusLogs", content: ticketStatusLog);
    }

    #endregion

    #region worklog

    public async Task<Worklog> GetWorklogAsync(int id)
    {
        return await ExecuteWithDataStateAsync<Worklog>(HttpMethod.Get, $"worklogs/{id}");
    }

    public async Task<Page<Worklog>> GetWorklogsAsync(int pageSize, int pageIndex, string? filter = null)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "pageSize", pageSize.ToString() },
            { "pageIndex", pageIndex.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(filter))
        {
            queryParams.Add("filter", filter);
        }

        return await ExecuteWithDataStateAsync<Page<Worklog>>(HttpMethod.Get, "worklogs", queryParams);
    }

    public async Task<Worklog> SaveWorklogAsync(Worklog ticket)
    {
        return await ExecuteWithDataStateAsync<Worklog>(HttpMethod.Post, "worklogs", content: ticket);
    }

    public async Task DeleteWorklogAsync(int id)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"worklogs/{id}");
    }

    #endregion

    private async Task<HttpResponseMessage> ExecuteWithDataStateAsync(HttpMethod method, string endpoint, Dictionary<string, string>? queryParams = null, object? content = null)
        => await ExecuteWithDataStateAsync<HttpResponseMessage>(method, endpoint, queryParams, content);

    private async Task<T> ExecuteWithDataStateAsync<T>(HttpMethod method, string endpoint, Dictionary<string, string>? queryParams = null, object? content = null)
    {
        _dataStateService.StartOperation();

        try
        {
            using var client = CreateClient();

            var api = $"{client.BaseAddress}{endpoint}{GetQueryString(queryParams)}";

            if (content is null)
            {
                _logger.LogInformation("Anfrage {Method} {API}", method.Method, api);
            }
            else
            {
                _logger.LogInformation("Anfrage {Method} {API} {@Content}", method.Method, api, content);
            }

            var jsonContent = JsonContent.Create(content);

            var res = await client.SendAsync(new(method, $"{endpoint}{GetQueryString(queryParams, true)}") { Content = jsonContent });

            var statusCode = (int)res.StatusCode;

            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch
            {
                var error = (await res.Content.ReadAsStringAsync()).Trim('"');

                _logger.LogError("Antwort {Methode} {API} {StatusCode} {Error}", method.Method, api, statusCode, error);
                _dataStateService.SetError(error);

                throw;
            }

            if (typeof(T) == typeof(HttpResponseMessage))
            {
                _logger.LogInformation("Antwort {Methode} {API} {StatusCode}", method.Method, api, statusCode);
                return (T)(object)res;
            }

            var resContent = (await res.Content.ReadFromJsonAsync<T>())!;

            _logger.LogInformation("Antwort {Methode} {API} {StatusCode} {@Content}", method.Method, api, statusCode, resContent);

            return resContent;
        }
        finally
        {
            _dataStateService.EndOperation();
        }
    }

    private static string GetQueryString(Dictionary<string, string>? parameters, bool escape = false)
    {
        if (parameters is null || parameters.Count == 0)
        {
            return string.Empty;
        }

        return "?" + string.Join('&', parameters.Select(x => $"{x.Key}={(escape ? Uri.EscapeDataString(x.Value) : x.Value)}"));
    }
}
