using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.Home;

public class HomeViewModel(IDataService dataService, IPopupService popupService) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly IPopupService _popupService = popupService;

    public bool LoadOvertime { get; set => SetValue(ref field, value); } = true;

    public OvertimeInfo Overtime { get; set => SetValue(ref field, value); } = new();

    public bool LoadCalendarStatistics { get; set => SetValue(ref field, value); } = true;

    public IDictionary<CalendarEntryType, int> CalendarStatistics { get; set => SetValue(ref field, value); } = Enum.GetValues<CalendarEntryType>().ToDictionary(x => x, _ => 0);

    public int SelectedYear { get; set => SetValue(ref field, value); } = DateTimeOffset.Now.Year;

    public async Task OnSelectedYearChanged()
    {
        await Task.WhenAll(
            LoadCalendarStatisticsAsync(),
            LoadWorkTimesAsync(),
            LoadAbsencesAsync(),
            LoadHolidaysAsync());
    }

    public DateOnly LoadDataFrom => new(SelectedYear - 1, 12, 1);

    public DateOnly LoadDataTo => new(SelectedYear + 1, 1, 31);

    public string SelectedFederalState { get; set => SetValue(ref field, value); } = "DE-HE";

    public async Task OnSelectedFederalStateChanged()
    {
        await LoadHolidaysAsync();
    }

    public bool LoadCalendar => LoadWorkTimes || LoadAbsences || LoadHolidays;

    public bool LoadWorkTimes
    {
        get;
        set
        {
            if (SetValue(ref field, value))
            {
                OnPropertyChanged(nameof(LoadCalendar));
            }
        }
    } = true;

    public IEnumerable<WorkTime> WorkTimes { get; set => SetValue(ref field, value); } = [];

    public bool LoadAbsences
    {
        get => field;
        set
        {
            if (SetValue(ref field, value))
            {
                OnPropertyChanged(nameof(LoadCalendar));
            }
        }
    } = true;

    public IEnumerable<Absence> Absences { get; set => SetValue(ref field, value); } = [];

    public bool LoadHolidays
    {
        get => field;
        set
        {
            if (SetValue(ref field, value))
            {
                OnPropertyChanged(nameof(LoadCalendar));
            }
        }
    } = true;

    public IEnumerable<Holiday> Holidays { get; set => SetValue(ref field, value); } = [];

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

    public async Task LoadCalendarStatisticsAsync()
    {
        LoadCalendarStatistics = true;

        try
        {
            CalendarStatistics = await _dataService.GetCalendarStaticsAsync(SelectedYear);
        }
        catch
        {
            _popupService.Error($"Fehler beim Laden der Kalendarstatistiken für {SelectedYear}!");

            Dictionary<CalendarEntryType, int> calendarStatistics = [];

            foreach (var type in Enum.GetValues<CalendarEntryType>())
            {
                calendarStatistics[type] = 0;
            }

            CalendarStatistics = calendarStatistics;
        }
        finally
        {
            LoadCalendarStatistics = false;
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
