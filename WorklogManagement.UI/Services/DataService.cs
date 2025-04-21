using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Services;

public interface IDataService
{
    #region calendar

    Task<OvertimeInfo> GetOvertimeAsync(CancellationToken cancellationToken = default);

    Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState, CancellationToken cancellationToken = default);

    Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);

    Task<List<DateOnly>> GetDatesWithWorkTimesAsync(CancellationToken cancellationToken = default);

    Task<WorkTime> SaveWorkTimeAsync(WorkTime workTime, CancellationToken cancellationToken = default);

    Task DeleteWorkTimeAsync(int id, CancellationToken cancellationToken = default);

    Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);

    Task<List<DateOnly>> GetDatesWithAbsencesAsync(CancellationToken cancellationToken = default);

    Task<Absence> SaveAbsenceAsync(Absence absence, CancellationToken cancellationToken = default);

    Task DeleteAbsenceAsync(int id, CancellationToken cancellationToken = default);

    #endregion

    #region ticket

    Task<Ticket> GetTicketAsync(int id, CancellationToken cancellationToken = default);

    Task<Page<Ticket>> GetTicketsAsync(int pageSize, int pageIndex, string? filter = null, CancellationToken cancellationToken = default);

    Task<Ticket> SaveTicketAsync(Ticket ticket, CancellationToken cancellationToken = default);

    Task DeleteTicketAsync(int id, CancellationToken cancellationToken = default);

    Task<List<TicketStatusLog>> GetTicketStatusLogsAsync(int ticketId, CancellationToken cancellationToken = default);

    Task<TicketStatusLog> SaveTicketStatusLogAsync(TicketStatusLog ticketStatusLog, CancellationToken cancellationToken = default);

    #endregion

    #region worklog

    Task<Worklog> GetWorklogAsync(int id, CancellationToken cancellationToken = default);

    Task<Page<Worklog>> GetWorklogsAsync(int pageSize, int pageIndex, string? filter = null, CancellationToken cancellationToken = default);

    Task<Worklog> SaveWorklogAsync(Worklog worklog, CancellationToken cancellationToken = default);

    Task DeleteWorklogAsync(int id, CancellationToken cancellationToken = default);

    #endregion
}

public class DataService(ILoggerService<DataService> logger, IHttpClientFactory httpClientFactory, IGlobalDataStateService dataStateService) : IDataService
{
    private readonly ILoggerService<DataService> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IGlobalDataStateService _dataStateService = dataStateService;

    private HttpClient CreateClient() => _httpClientFactory.CreateClient(nameof(WorklogManagement));

    #region calendar

    public async Task<OvertimeInfo> GetOvertimeAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<OvertimeInfo>(HttpMethod.Get, "statistics/overtime", cancellationToken: cancellationToken);
    }

    public async Task<List<Holiday>> GetHolidaysAsync(DateOnly from, DateOnly to, string federalState, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "from", from.ToString("yyyy-MM-dd") },
            { "to", to.ToString("yyyy-MM-dd") }
        };

        return await ExecuteWithDataStateAsync<List<Holiday>>(HttpMethod.Get, $"holidays/{federalState}", queryParams, cancellationToken: cancellationToken);
    }

    public async Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "pageSize", "0" },
            { "filter", $@"Date >= ""{from:yyyy-MM-dd}"" && Date <= ""{to:yyyy-MM-dd}""" }
        };

        var page = await ExecuteWithDataStateAsync<Page<WorkTime>>(HttpMethod.Get, "worktimes", queryParams, cancellationToken: cancellationToken);

        return [.. page.Items];
    }

    public async Task<List<DateOnly>> GetDatesWithWorkTimesAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<List<DateOnly>>(HttpMethod.Get, "worktimes/dates", cancellationToken: cancellationToken);
    }

    public async Task<WorkTime> SaveWorkTimeAsync(WorkTime workTime, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<WorkTime>(HttpMethod.Post, "worktimes", content: workTime, cancellationToken: cancellationToken);
    }

    public async Task DeleteWorkTimeAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"worktimes/{id}", cancellationToken: cancellationToken);
    }

    public async Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "pageSize", "0" },
            { "filter", $@"Date >= ""{from:yyyy-MM-dd}"" && Date <= ""{to:yyyy-MM-dd}""" }
        };

        var page = await ExecuteWithDataStateAsync<Page<Absence>>(HttpMethod.Get, "absences", queryParams, cancellationToken: cancellationToken);

        return [.. page.Items];
    }

    public async Task<List<DateOnly>> GetDatesWithAbsencesAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<List<DateOnly>>(HttpMethod.Get, "absences/dates", cancellationToken: cancellationToken);
    }

    public async Task<Absence> SaveAbsenceAsync(Absence absence, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<Absence>(HttpMethod.Post, "absences", content: absence, cancellationToken: cancellationToken);
    }

    public async Task DeleteAbsenceAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"absences/{id}", cancellationToken: cancellationToken);
    }

    #endregion

    #region ticket

    public async Task<Ticket> GetTicketAsync(int id, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<Ticket>(HttpMethod.Get, $"tickets/{id}", cancellationToken: cancellationToken);
    }

    public async Task<Page<Ticket>> GetTicketsAsync(int pageSize, int pageIndex, string? filter = null, CancellationToken cancellationToken = default)
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

        return await ExecuteWithDataStateAsync<Page<Ticket>>(HttpMethod.Get, "tickets", queryParams, cancellationToken: cancellationToken);
    }

    public async Task<Ticket> SaveTicketAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<Ticket>(HttpMethod.Post, "tickets", content: ticket, cancellationToken: cancellationToken);
    }

    public async Task DeleteTicketAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"tickets/{id}", cancellationToken: cancellationToken);
    }

    public async Task<List<TicketStatusLog>> GetTicketStatusLogsAsync(int ticketId, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> queryParams = new()
        {
            { "ticketId", ticketId.ToString() }
        };

        var page = await ExecuteWithDataStateAsync<Page<TicketStatusLog>>(HttpMethod.Get, "ticketStatusLogs", queryParams, cancellationToken: cancellationToken);

        return [.. page.Items];
    }

    public async Task<TicketStatusLog> SaveTicketStatusLogAsync(TicketStatusLog ticketStatusLog, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<TicketStatusLog>(HttpMethod.Post, "ticketStatusLogs", content: ticketStatusLog, cancellationToken: cancellationToken);
    }

    #endregion

    #region worklog

    public async Task<Worklog> GetWorklogAsync(int id, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<Worklog>(HttpMethod.Get, $"worklogs/{id}", cancellationToken: cancellationToken);
    }

    public async Task<Page<Worklog>> GetWorklogsAsync(int pageSize, int pageIndex, string? filter = null, CancellationToken cancellationToken = default)
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

        return await ExecuteWithDataStateAsync<Page<Worklog>>(HttpMethod.Get, "worklogs", queryParams, cancellationToken: cancellationToken);
    }

    public async Task<Worklog> SaveWorklogAsync(Worklog ticket, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithDataStateAsync<Worklog>(HttpMethod.Post, "worklogs", content: ticket, cancellationToken: cancellationToken);
    }

    public async Task DeleteWorklogAsync(int id, CancellationToken cancellationToken = default)
    {
        await ExecuteWithDataStateAsync(HttpMethod.Delete, $"worklogs/{id}", cancellationToken: cancellationToken);
    }

    #endregion

    private async Task<HttpResponseMessage> ExecuteWithDataStateAsync(HttpMethod method, string endpoint, Dictionary<string, string>? queryParams = null, object? content = null, CancellationToken cancellationToken = default)
        => await ExecuteWithDataStateAsync<HttpResponseMessage>(method, endpoint, queryParams, content, cancellationToken);

    private async Task<T> ExecuteWithDataStateAsync<T>(HttpMethod method, string endpoint, Dictionary<string, string>? queryParams = null, object? content = null, CancellationToken cancellationToken = default)
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

            var res = await client.SendAsync(new(method, $"{endpoint}{GetQueryString(queryParams, true)}") { Content = jsonContent }, cancellationToken);

            var statusCode = (int)res.StatusCode;

            try
            {
                res.EnsureSuccessStatusCode();
            }
            catch
            {
                var error = (await res.Content.ReadAsStringAsync(cancellationToken)).Trim('"');

                _logger.LogError("Antwort {Methode} {API} {StatusCode} {Error}", method.Method, api, statusCode, error);
                _dataStateService.SetError(error);

                throw;
            }

            if (typeof(T) == typeof(HttpResponseMessage))
            {
                _logger.LogInformation("Antwort {Methode} {API} {StatusCode}", method.Method, api, statusCode);
                return (T)(object)res;
            }

            var resContent = (await res.Content.ReadFromJsonAsync<T>(cancellationToken))!;

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
