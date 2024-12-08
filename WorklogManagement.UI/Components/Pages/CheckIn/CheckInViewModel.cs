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

    public void OpenDialog()
    {
        IsDialogOpen = true;
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

    private ICollection<DateOnly> _datesWithWorkTimes = [];
    public ICollection<DateOnly> DatesWithWorkTimes
    {
        get => _datesWithWorkTimes;
        set => SetValue(ref _datesWithWorkTimes, value);
    }

    private ICollection<DateOnly> _datesWithAbsences = [];
    public ICollection<DateOnly> DatesWithAbsences
    {
        get => _datesWithAbsences;
        set => SetValue(ref _datesWithAbsences, value);
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
            LoadDatesWithWorkTimesAsync(),
            LoadDatesWithAbsencesAsync(),
            LoadWorkTimesAndAbsencesAsync()
        ]);
    }

    public async Task LoadDatesWithWorkTimesAsync()
    {
        try
        {
            DatesWithWorkTimes = await _dataService.GetDatesWithWorkTimesAsync();
        }
        catch
        {
            // hat nur Auswirkungen auf die Darstellung, nicht auf die Funktionalität
        }
    }

    public async Task LoadDatesWithAbsencesAsync()
    {
        try
        {
            DatesWithAbsences = await _dataService.GetDatesWithAbsencesAsync();
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
        var savedWorkTime = await _dataService.SaveWorkTimeAsync(workTime);

        if (!WorkTimes.Any(x => x.Id == savedWorkTime.Id))
        {
            WorkTimes.Add(savedWorkTime);
            OnPropertyChanged(nameof(WorkTimes));
        }

        if (DatesWithAbsences.Contains(savedWorkTime.Date))
        {
            DatesWithAbsences.Add(savedWorkTime.Date);
            OnPropertyChanged(nameof(DatesWithAbsences));
        }
    }

    public async Task SaveAbsenceAsync(Absence absence)
    {
        var savedAbsence = await _dataService.SaveAbsenceAsync(absence);

        if (!Absences.Any(x => x.Id == savedAbsence.Id))
        {
            Absences.Add(savedAbsence);
            OnPropertyChanged(nameof(Absences));
        }

        if (DatesWithAbsences.Contains(savedAbsence.Date))
        {
            DatesWithAbsences.Add(savedAbsence.Date);
            OnPropertyChanged(nameof(DatesWithAbsences));
        }
    }
}
