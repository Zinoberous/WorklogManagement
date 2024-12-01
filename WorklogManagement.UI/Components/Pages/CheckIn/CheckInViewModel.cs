using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public class CheckInViewModel(IDataService dataService, INavigator navigator) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly INavigator _navigator = navigator;

    private bool _dialogIsOpen = false;
    public bool IsDialogOpen
    {
        get => _dialogIsOpen;
        set => SetValue(ref _dialogIsOpen, value);
    }

    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetValue(ref _selectedDate, value))
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
            SelectedDate = initialDate.Value;
        }

        await LoadWorkTimesAndAbsencesAsync();
    }

    public async Task LoadWorkTimesAndAbsencesAsync()
    {
        IsLoading = true;

        try
        {
            WorkTimes = await _dataService.GetWorkTimesAsync(SelectedDate);
            Absences = await _dataService.GetAbsencesAsync(SelectedDate);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SaveWorkTimeAsync(WorkTime _)
    {
        // TODO: SaveWorkTimeAsync

        await Task.CompletedTask;
    }

    public async Task SaveAbsenceAsync(Absence _)
    {
        // TODO: SaveAbsenceAsync

        await Task.CompletedTask;
    }

    private async Task OnDateChangedAsync()
    {
        _navigator.UpdateQuery("date", $"{SelectedDate:yyyy-MM-dd}");

        await LoadWorkTimesAndAbsencesAsync();
    }
}
