using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public class CheckInViewModel(IDataService dataService, INavigator navigator) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly INavigator _navigator = navigator;

    private DateOnly _date = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly Date
    {
        get => _date;
        set
        {
            if (SetValue(ref _date, value))
            {
                _ = OnDateChangedAsync();
            }
        }
    }

    private bool _isLoading = true;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetValue(ref _isLoading, value);
    }

    private IEnumerable<WorkTime> _workTimes = [];
    public IEnumerable<WorkTime> WorkTimes
    {
        get => _workTimes;
        set => SetValue(ref _workTimes, value);
    }

    private IEnumerable<Absence> _absences = [];
    public IEnumerable<Absence> Absences
    {
        get => _absences;
        set => SetValue(ref _absences, value);
    }

    public async Task InitAsync(DateOnly? initialDate = null)
    {
        if (initialDate.HasValue)
        {
            Date = initialDate.Value;
        }

        await LoadWorkTimesAndAbsencesAsync();
    }

    public async Task LoadWorkTimesAndAbsencesAsync()
    {
        IsLoading = true;

        try
        {
            WorkTimes = await _dataService.GetWorkTimesAsync(Date);
            Absences = await _dataService.GetAbsencesAsync(Date);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OnDateChangedAsync()
    {
        _navigator.UpdateQuery("date", $"{Date:yyyy-MM-dd}");

        await LoadWorkTimesAndAbsencesAsync();
    }
}
