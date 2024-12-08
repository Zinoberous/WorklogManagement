using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public class CheckInViewModel(IDataService dataService, INavigator navigator, INotifier notifier) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly INavigator _navigator = navigator;
    private readonly INotifier _notifier = notifier;

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
        set => SetValue(ref _selectedDate, value);
    }

    public async Task OnSelectedDateChanged()
    {
        _navigator.UpdateQuery("date", $"{SelectedDate:yyyy-MM-dd}");

        await LoadWorkTimesAndAbsencesAsync();
    }

    private ICollection<DateOnly> _datesWithEntries = [];
    public ICollection<DateOnly> DatesWithEntries
    {
        get => _datesWithEntries;
        set => SetValue(ref _datesWithEntries, value);
    }

    private bool _isLoading = true;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetValue(ref _isLoading, value);
    }

    private ICollection<WorkTime> _workTimes = [];
    public ICollection<WorkTime> WorkTimes
    {
        get => _workTimes;
        set => SetValue(ref _workTimes, value);
    }

    private ICollection<Absence> _absences = [];
    public ICollection<Absence> Absences
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

        await Task.WhenAll([
            LoadDatesWithEntriesAsync(),
            LoadWorkTimesAndAbsencesAsync()
        ]);
    }

    public async Task LoadDatesWithEntriesAsync()
    {
        try
        {
            DatesWithEntries = await _dataService.GetDatesWithEntriesAsync();
        }
        catch
        {
            // hat nur Auswirkungen auf die Darstellung, nicht auf die Funktionalität
        }
    }

    public async Task LoadWorkTimesAndAbsencesAsync()
    {
        IsLoading = true;

        try
        {
            var workTimes = await _dataService.GetWorkTimesAsync(SelectedDate);
            var absences = await _dataService.GetAbsencesAsync(SelectedDate);

            // nachträgliche Zuweisung der Werte, so werden die Werte entweder ganz oder gar nicht angezeigt, wenn ein Fehler auftritt
            WorkTimes = workTimes;
            Absences = absences;
        }
        catch (Exception ex)
        {
            await _notifier.NotifyErrorAsync("Fehler beim Laden der Kalendareinträge!", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SaveWorkTimeAsync(WorkTime workTime)
    {
        // TODO: SaveWorkTimeAsync

        if (!WorkTimes.Any(x => x.Id == workTime.Id))
        {
            WorkTimes.Add(workTime);
            OnPropertyChanged(nameof(WorkTimes));
        }

        if (DatesWithEntries.Contains(workTime.Date))
        {
            DatesWithEntries.Add(workTime.Date);
            OnPropertyChanged(nameof(DatesWithEntries));
        }

        await Task.CompletedTask;
    }

    public async Task SaveAbsenceAsync(Absence absence)
    {
        // TODO: SaveAbsenceAsync

        if (!Absences.Any(x => x.Id == absence.Id))
        {
            Absences.Add(absence);
            OnPropertyChanged(nameof(Absences));
        }

        if (DatesWithEntries.Contains(absence.Date))
        {
            DatesWithEntries.Add(absence.Date);
            OnPropertyChanged(nameof(DatesWithEntries));
        }

        await Task.CompletedTask;
    }
}
