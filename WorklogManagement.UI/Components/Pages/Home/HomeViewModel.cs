using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.Home;

public class HomeViewModel(IDataService dataService, IPopupService popupService) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly IPopupService _popupService = popupService;

    private bool _loadOvertime = true;
    public bool LoadOvertime
    {
        get => _loadOvertime;
        set => SetValue(ref _loadOvertime, value);
    }

    private OvertimeInfo _overtime = new();
    public OvertimeInfo Overtime
    {
        get => _overtime;
        set => SetValue(ref _overtime, value);
    }

    private int _selectedYear = DateTimeOffset.Now.Year;
    public int SelectedYear
    {
        get => _selectedYear;
        set => SetValue(ref _selectedYear, value);
    }

    public async Task OnSelectedYearChanged()
    {
        await Task.WhenAll([
            LoadWorkTimesAsync(),
            LoadAbsencesAsync(),
            LoadHolidaysAsync(),
        ]);
    }

    public DateOnly LoadDataFrom => new(SelectedYear - 1, 12, 1);
    public DateOnly LoadDataTo => new(SelectedYear + 1, 1, 31);

    private string _selectedFederalState = "DE-HE";
    public string SelectedFederalState
    {
        get => _selectedFederalState;
        set => SetValue(ref _selectedFederalState, value);
    }

    public async Task OnSelectedFederalStateChanged()
    {
        await LoadHolidaysAsync();
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
        catch
        {
            _popupService.Error("Fehler beim Laden der Überstunden!");
        }
        finally
        {
            LoadOvertime = false;
        }
    }

    public async Task LoadWorkTimesAsync()
    {
        LoadWorkTimes = true;

        try
        {
            WorkTimes = await _dataService.GetWorkTimesAsync(LoadDataFrom, LoadDataTo);
        }
        catch
        {
            _popupService.Error("Fehler beim Laden der Arbeitszeiten!");
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
            Absences = await _dataService.GetAbsencesAsync(LoadDataFrom, LoadDataTo);
        }
        catch
        {
            _popupService.Error("Fehler beim Laden der Abwesenheiten!");
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
            Holidays = await _dataService.GetHolidaysAsync(LoadDataFrom, LoadDataTo, SelectedFederalState);
        }
        catch
        {
            _popupService.Error("Fehler beim Laden der Feiertage!");
        }
        finally
        {
            LoadHolidays = false;
        }
    }
}
