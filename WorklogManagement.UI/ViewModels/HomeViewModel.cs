using Microsoft.JSInterop;
using Radzen;
using WorklogManagement.Service;
using WorklogManagement.Service.Enums;
using WorklogManagement.Service.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.ViewModels;

public class HomeViewModel(IWorklogManagementService service, IHttpClientFactory httpClientFactory, NotificationService notificationService, IJSRuntime jsRuntime) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly NotificationService _notificationService = notificationService;
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    private bool _loadOvertime = true;
    public bool LoadOvertime
    {
        get => _loadOvertime;
        set => SetValue(ref _loadOvertime, value);
    }

    private Exception? _loadOvertimeError;
    public Exception? LoadOvertimeError
    {
        get => _loadOvertimeError;
        set => SetValue(ref _loadOvertimeError, value);
    }

    private OvertimeInfo? _overtime;
    public OvertimeInfo? Overtime
    {
        get => _overtime;
        set => SetValue(ref _overtime, value);
    }

    private bool _loadCalendarStatistics = true;
    public bool LoadCalendarStatistics
    {
        get => _loadCalendarStatistics;
        set => SetValue(ref _loadCalendarStatistics, value);
    }

    private Exception? _loadCalendarStatisticsError;
    public Exception? LoadCalendarStatisticsError
    {
        get => _loadCalendarStatisticsError;
        set => SetValue(ref _loadCalendarStatisticsError, value);
    }

    private Dictionary<CalendarEntryType, int> _calendarStatistics = [];
    public Dictionary<CalendarEntryType, int> CalendarStatistics
    {
        get => _calendarStatistics;
        set => SetValue(ref _calendarStatistics, value);
    }

    private bool _loadTicketStatistics = true;
    public bool LoadTicketStatistics
    {
        get => _loadTicketStatistics;
        set => SetValue(ref _loadTicketStatistics, value);
    }

    private Exception? _loadTicketStatisticsError;
    public Exception? LoadTicketStatisticsError
    {
        get => _loadTicketStatisticsError;
        set => SetValue(ref _loadTicketStatisticsError, value);
    }

    private Dictionary<TicketStatus, int> _ticketStatistics = [];
    public Dictionary<TicketStatus, int> TicketStatistics
    {
        get => _ticketStatistics;
        set => SetValue(ref _ticketStatistics, value);
    }

    private int _selectedYear = DateTimeOffset.Now.Year;
    public int SelectedYear
    {
        get => _selectedYear;
        set
        {
            if (SetValue(ref _selectedYear, value))
            {
                _ = OnSelectedYearChangedAsync();
            }
        }
    }

    private string _selectedFederalState = "DE-HE";
    public string SelectedFederalState
    {
        get => _selectedFederalState;
        set
        {
            if (SetValue(ref _selectedFederalState, value))
            {
                _ = OnSelectedFederalStateChangedAsync();
            }
        }
    }

    public bool LoadCalendar => LoadWorkTimes || LoadAbsences || LoadHolidays;

    private bool _loadWorkTimes = true;
    public bool LoadWorkTimes
    {
        get => _loadWorkTimes;
        set
        {
            if (SetValue(ref _loadWorkTimes, value))
            {
                OnPropertyChanged(nameof(LoadCalendar));
            }
        }
    }

    private IEnumerable<WorkTime> _workTimes = [];
    public IEnumerable<WorkTime> WorkTimes
    {
        get => _workTimes;
        set => SetValue(ref _workTimes, value);
    }

    private bool _loadAbsences = true;
    public bool LoadAbsences
    {
        get => _loadAbsences;
        set
        {
            if (SetValue(ref _loadAbsences, value))
            {
                OnPropertyChanged(nameof(LoadCalendar));
            }
        }
    }

    private IEnumerable<Absence> _absences = [];
    public IEnumerable<Absence> Absences
    {
        get => _absences;
        set => SetValue(ref _absences, value);
    }

    private bool _loadHolidays = true;
    public bool LoadHolidays
    {
        get => _loadHolidays;
        set
        {
            if (SetValue(ref _loadHolidays, value))
            {
                OnPropertyChanged(nameof(LoadCalendar));
            }
        }
    }

    private IEnumerable<Holiday> _holidays = [];
    public IEnumerable<Holiday> Holidays
    {
        get => _holidays;
        set => SetValue(ref _holidays, value);
    }

    public async Task LoadOvertimeAsync()
    {
        LoadOvertime = true;

        try
        {
            Overtime = await _service.GetOvertimeAsync();
        }
        catch (Exception ex)
        {
            LoadOvertimeError = ex;
        }
        finally
        {
            LoadOvertime = false;
        }
    }

    public async Task LoadCalendarStatisticsAsync()
    {
        LoadCalendarStatistics = true;

        try
        {
            // TODO: GetCalendarStaticsAsync() => over all years
            CalendarStatistics = await _service.GetCalendarStaticsAsync(SelectedYear);
        }
        catch (Exception ex)
        {
            LoadCalendarStatisticsError = ex;
        }
        finally
        {
            LoadCalendarStatistics = false;
        }
    }

    public async Task LoadTicketStatisticsAsync()
    {
        LoadTicketStatistics = true;

        try
        {
            TicketStatistics = await _service.GetTicketStatisticsAsync();
        }
        catch (Exception ex)
        {
            LoadTicketStatisticsError = ex;
        }
        finally
        {
            LoadTicketStatistics = false;
        }
    }

    public async Task LoadWorkTimesAsync()
    {
        LoadWorkTimes = true;

        try
        {
            WorkTimes = await _service.GetWorkTimesOfYearAsync(SelectedYear);
        }
        catch (Exception ex)
        {
            _notificationService.Notify(NotificationSeverity.Error, "Fehler beim Laden der Arbeitszeiten!");

            await _jsRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            LoadWorkTimes = false;
        }
    }

    public async Task LoadAbsencesAsync()
    {
        LoadAbsences = true;

        try
        {
            Absences = await _service.GetAbsencesOfYearAsyncAsync(SelectedYear);
        }
        catch (Exception ex)
        {
            _notificationService.Notify(NotificationSeverity.Error, "Fehler beim Laden der Abwesenheiten!");

            await _jsRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            LoadAbsences = false;
        }
    }

    public async Task LoadHolidaysAsync()
    {
        LoadHolidays = true;

        try
        {
            using var client = _httpClientFactory.CreateClient();

            var res = await client.GetAsync($"https://date.nager.at/api/v3/PublicHolidays/{SelectedYear}/DE");

            res.EnsureSuccessStatusCode();

            var holidays = await res.Content.ReadFromJsonAsync<IEnumerable<HolidayDto>>();

            Holidays = holidays?
                .Where(h => h.Counties == null || h.Counties.Contains(SelectedFederalState))
                .Select(x => new Holiday { Date = x.Date, Name = x.LocalName })
                ?? [];
        }
        catch (Exception ex)
        {
            _notificationService.Notify(NotificationSeverity.Error, "Fehler beim Laden der Feiertage!");

            await _jsRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            LoadHolidays = false;
        }
    }

    private async Task OnSelectedYearChangedAsync()
    {
        await LoadCalendarStatisticsAsync();
        await LoadWorkTimesAsync();
        await LoadAbsencesAsync();
    }

    private async Task OnSelectedFederalStateChangedAsync()
    {
        await LoadHolidaysAsync();
    }
}
