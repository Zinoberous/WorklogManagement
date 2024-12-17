using Radzen;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Common;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public class CheckInViewModel(IDataService dataService, INavigationService navigationService, IPopupService popupService) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IPopupService _popupService = popupService;

    private bool _dialogIsOpen = false;
    public bool IsDialogOpen
    {
        get => _dialogIsOpen;
        set => SetValue(ref _dialogIsOpen, value);
    }

    public IEnumerable<string> UsedTypes =>
        WorkTimes.Select(x => Constant.WorkTimeLabels[x.Type])
        .Concat(Absences.Select(x => Constant.AbsenceLabels[x.Type]))
        .ToArray();

    public bool CreateNewDisabled =>
        Constant.WorkTimeLabels.Values
        .Concat(Constant.AbsenceLabels.Values)
        .Count() == UsedTypes.Count();

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
        _navigationService.UpdateQuery("date", $"{SelectedDate:yyyy-MM-dd}");

        await LoadCalendarEntriesAsync();
    }

    private IEnumerable<DateOnly> _datesWithWorkTimes = [];
    public IEnumerable<DateOnly> DatesWithWorkTimes
    {
        get => _datesWithWorkTimes;
        set => SetValue(ref _datesWithWorkTimes, value);
    }

    private IEnumerable<DateOnly> _datesWithAbsences = [];
    public IEnumerable<DateOnly> DatesWithAbsences
    {
        get => _datesWithAbsences;
        set => SetValue(ref _datesWithAbsences, value);
    }

    private bool _loadCalendarEntries = true;
    public bool LoadCalendarEntries
    {
        get => _loadCalendarEntries;
        set => SetValue(ref _loadCalendarEntries, value);
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

        await Task.WhenAll([
            LoadDatesWithWorkTimesAsync(),
            LoadDatesWithAbsencesAsync(),
            LoadCalendarEntriesAsync()
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

    public async Task LoadCalendarEntriesAsync()
    {
        LoadCalendarEntries = true;

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
            await _popupService.Error("Fehler beim Laden der Kalendareinträge!", ex);
        }
        finally
        {
            LoadCalendarEntries = false;
        }
    }

    public async Task<bool> SaveWorkTimeAsync(WorkTime workTime)
    {
        WorkTime savedWorkTime;

        try
        {
            savedWorkTime = await _dataService.SaveWorkTimeAsync(workTime);
        }
        catch (Exception ex)
        {
            await _popupService.Error("Fehler beim Speichern der Anwesenheit!", ex);
            return false;
        }

        // nur wenn neuer Eintrag
        if (savedWorkTime.Id != workTime.Id)
        {
            _popupService.Success($"{Constant.WorkTimeLabels[workTime.Type]}-Eintrag wurde gespeichert");
        }

        WorkTimes = WorkTimes.Where(x => x.Id != savedWorkTime.Id).Append(savedWorkTime).ToArray();
        DatesWithWorkTimes = DatesWithWorkTimes.Append(SelectedDate).Distinct().ToArray();

        return true;
    }

    public async Task<bool> DeleteWorkTimeAsync(WorkTime workTime)
    {
        if (!(await _popupService.Confim("Eintrag löschen", "Möchtest du den Eintrag wirklich löschen?")))
        {
            return false;
        }

        try
        {
            await _dataService.DeleteWorkTimeAsync(workTime.Id);
        }
        catch (Exception ex)
        {
            await _popupService.Error("Fehler beim Löschen der Anwesenheit!", ex);
            return false;
        }

        WorkTimes = WorkTimes.Where(x => x.Id != workTime.Id).ToArray();

        if (!WorkTimes.Any())
        {
            DatesWithWorkTimes = DatesWithWorkTimes.Where(x => x != SelectedDate).ToArray();
        }

        _popupService.Info($"{Constant.WorkTimeLabels[workTime.Type]}-Eintrag wurde gelöscht");

        return true;
    }

    public async Task<bool> SaveAbsenceAsync(Absence absence)
    {
        Absence savedAbsence;

        try
        {
            savedAbsence = await _dataService.SaveAbsenceAsync(absence);
        }
        catch (Exception ex)
        {
            await _popupService.Error("Fehler beim Speichern der Abwesenheit!", ex);
            return false;
        }

        // nur wenn neuer Eintrag
        if (savedAbsence.Id != absence.Id)
        {
            _popupService.Success($"{Constant.AbsenceLabels[absence.Type]}-Eintrag wurde gespeichert");
        }

        Absences = Absences.Where(x => x.Id != savedAbsence.Id).Append(savedAbsence).ToArray();
        DatesWithAbsences = DatesWithAbsences.Append(SelectedDate).Distinct().ToArray();

        return true;
    }

    public async Task<bool> DeleteAbsenceAsync(Absence absence)
    {
        if (!(await _popupService.Confim("Eintrag löschen", "Möchtest du den Eintrag wirklich löschen?")))
        {
            return false;
        }

        try
        {
            await _dataService.DeleteAbsenceAsync(absence.Id);
        }
        catch (Exception ex)
        {
            await _popupService.Error("Fehler beim Löschen der Abwesenheit!", ex);
            return false;
        }

        Absences = Absences.Where(x => x.Id != absence.Id).ToArray();

        if (!Absences.Any())
        {
            DatesWithAbsences = DatesWithAbsences.Where(x => x != SelectedDate).ToArray();
        }

        _popupService.Info($"{Constant.AbsenceLabels[absence.Type]}-Eintrag wurde gelöscht");

        return true;
    }
}
