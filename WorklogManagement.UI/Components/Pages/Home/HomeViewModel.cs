using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.Components.Pages.Home;

public class HomeViewModel(IDataService dataService) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;

    private bool _loadOvertime = true;
    public bool LoadOvertime
    {
        get => _loadOvertime;
        set => SetValue(ref _loadOvertime, value);
    }

    private OvertimeInfo _overtime = new() { TotalMinutes = 0, OfficeMinutes = 0, MobileMinutes = 0 };
    public OvertimeInfo Overtime
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

    private Dictionary<CalendarEntryType, int> _calendarStatisticsYear = Enum.GetValues<CalendarEntryType>().ToDictionary(x => x, _ => 0);
    public Dictionary<CalendarEntryType, int> CalendarStatisticsYear
    {
        get => _calendarStatisticsYear;
        set => SetValue(ref _calendarStatisticsYear, value);
    }

    private Dictionary<CalendarEntryType, int> _calendarStatisticsAll = Enum.GetValues<CalendarEntryType>().ToDictionary(x => x, _ => 0);
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

    private Dictionary<TicketStatus, int> _ticketStatistics = Enum.GetValues<TicketStatus>().ToDictionary(x => x, _ => 0);
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
            Overtime = await _dataService.GetOvertimeAsync();
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
            CalendarStatisticsYear = await _dataService.GetCalendarStaticsAsync(SelectedYear);
            CalendarStatisticsAll = await _dataService.GetCalendarStaticsAsync();
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
            TicketStatistics = await _dataService.GetTicketStatisticsAsync();
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
            WorkTimes = await _dataService.GetWorkTimesAsync(new(SelectedYear, 1, 1), new(SelectedYear, 12, 31));
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
            Absences = await _dataService.GetAbsencesAsync(new(SelectedYear, 1, 1), new(SelectedYear, 12, 31));
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
            Holidays = await _dataService.GetHolidaysAsync(SelectedYear, SelectedFederalState);
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
