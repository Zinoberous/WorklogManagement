using Radzen;
using WorklogManagement.Service;
using WorklogManagement.Service.Enums;
using WorklogManagement.Service.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.ViewModels;

public class HomeViewModel(IWorklogManagementService service, IHttpClientFactory httpClientFactory, INotifier notifier) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly INotifier _notifier = notifier;

    private bool _loadOvertime = true;
    public bool LoadOvertime
    {
        get => _loadOvertime;
        set => SetValue(ref _loadOvertime, value);
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

    private Dictionary<CalendarEntryType, int> _calendarStatisticsYear = [];
    public Dictionary<CalendarEntryType, int> CalendarStatisticsYear
    {
        get => _calendarStatisticsYear;
        set => SetValue(ref _calendarStatisticsYear, value);
    }

    private Dictionary<CalendarEntryType, int> _calendarStatisticsAll = [];
    public Dictionary<CalendarEntryType, int> CalendarStatisticsAll
    {
        get => _calendarStatisticsAll;
        set => SetValue(ref _calendarStatisticsAll, value);
    }

    private bool _loadTicketStatistics = true;
    public bool LoadTicketStatistics
    {
        get => _loadTicketStatistics;
        set => SetValue(ref _loadTicketStatistics, value);
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
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Überstunden!", ex);
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
            CalendarStatisticsYear = await _service.GetCalendarStaticsAsync(SelectedYear);
            CalendarStatisticsAll = await _service.GetCalendarStaticsAsync();
        }
        catch (Exception ex)
        {
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Kalendereinträge!", ex);
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
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Tickets!", ex);
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
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Arbeitszeiten!", ex);
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
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Abwesenheiten!", ex);
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
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Feiertage!", ex);
        }
        finally
        {
            LoadHolidays = false;
        }
    }

    private async Task OnSelectedYearChangedAsync()
    {
        await Task.WhenAll([
            LoadCalendarStatisticsAsync(),
            LoadWorkTimesAsync(),
            LoadAbsencesAsync(),
        ]);
    }

    private async Task OnSelectedFederalStateChangedAsync()
    {
        await LoadHolidaysAsync();
    }
}
