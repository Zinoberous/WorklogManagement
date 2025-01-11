using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.WorklogForm;

public class WorklogFormViewModel(IDataService dataService, INavigationService navigationService, IPopupService popupService) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IPopupService _popupService = popupService;

    private Worklog _worklog = null!;

    private bool _isLoading = true;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetValue(ref _isLoading, value);
    }

    public DateOnly Date
    {
        get => _worklog.Date;
        set => _ = SaveWorklogAsync(_worklog with { Date = value });
    }

    public TimeSpan TimeSpent
    {
        get => _worklog.TimeSpent;
        set => _ = SaveWorklogAsync(_worklog with { TimeSpent = value });
    }

    public string? Description
    {
        get => _worklog.Description;
        set => _ = SaveWorklogAsync(_worklog with { Description = value });
    }

    public IEnumerable<WorklogAttachment> Attachments
    {
        get => _worklog.Attachments;
        set => _ = SaveWorklogAsync(_worklog with { Attachments = value });
    }

    public async Task LoadWorklogAsync(int id)
    {
        IsLoading = true;

        try
        {
            //_worklog = await _dataService.GetWorklogAsync(id);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveWorklogAsync(Worklog worklog)
    {
        _worklog = worklog;

        //await _dataService.SaveWorklogAsync(worklog);
    }

    public async Task<bool> DeleteTicketAsync()
    {
        try
        {
            //await _dataService.DeleteWorklogAsync(_worklog.Id);
        }
        catch
        {
            _popupService.Error("Fehler beim Löschen!");
            return false;
        }

        _popupService.Info("Eintrag gelöscht!");
        _navigationService.NavigateToPage("/tracking");

        return true;
    }
}
