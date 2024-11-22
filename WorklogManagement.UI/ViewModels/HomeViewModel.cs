using Microsoft.JSInterop;
using Radzen;
using WorklogManagement.Service;
using WorklogManagement.Service.Models;

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
                _ = OnSelectedYearChangedAsync();
            }
        }
    }

    public bool LoadCalendar => LoadWorkTimes || LoadAbsences || LoadHolidays;

    private bool _loadCalendarError = true;
    public bool LoadCalendarError
    {
        get => _loadCalendarError;
        set => SetValue(ref _loadCalendarError, value);
    }

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

    // TODO: Holidays

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

    public async Task LoadWorkTimesAsync()
    {
        LoadWorkTimes = true;

        try
        {
            WorkTimes = await _service.GetWorkTimesOfYearAsync(SelectedYear);
        }
        catch (Exception ex)
        {
            LoadCalendarError = true;

            _notificationService.Notify(NotificationSeverity.Error, "Fehler beim Laden der Anwesenheiten.");

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
            LoadCalendarError = true;

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
        using var client = _httpClientFactory.CreateClient();

        var res = await client.GetAsync($"https://date.nager.at/api/v3/PublicHolidays/{SelectedYear}/DE");

        // TODO: Holiday = [];
    }

    private async Task OnSelectedYearChangedAsync()
    {
        await LoadWorkTimesAsync();
        await LoadAbsencesAsync();
    }

    private async Task OnSelectedFederalStateChangedAsync()
    {
        await LoadHolidaysAsync();
    }
}
