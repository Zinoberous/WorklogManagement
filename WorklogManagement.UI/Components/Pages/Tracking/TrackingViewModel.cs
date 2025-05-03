using Blazored.LocalStorage;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.Tracking;

public class TrackingViewModel(
    IDataService dataService,
    IPopupService popupService,
    ILocalStorageService localStorageService,
    INavigationService navigationService)
    : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly IPopupService _popupService = popupService;
    private readonly ILocalStorageService _localStorageService = localStorageService;
    private readonly INavigationService _navigationService = navigationService;

    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetValue(ref _selectedDate, value))
            {
                _ = OnSelectedDateChanged();
            }
        }
    }

    public async Task OnSelectedDateChanged()
    {
        _navigationService.UpdateQuery("date", $"{SelectedDate:yyyy-MM-dd}");
        await LoadWorklogsAsync();
    }

    private string _search = string.Empty;
    public string Search
    {
        get => _search;
        set
        {
            if (SetValue(ref _search, string.IsNullOrWhiteSpace(value) ? string.Empty : value))
            {
                _ = OnSearchChanged();
            }
        }
    }

    private async Task OnSearchChanged()
    {
        _navigationService.UpdateQuery("search", Search);
        await LoadWorklogsAsync();
    }

    private bool _loadWorklogs = true;
    public bool LoadWorklogs
    {
        get => _loadWorklogs;
        set => SetValue(ref _loadWorklogs, value);
    }

    private IEnumerable<Worklog> _worklogs = [];
    public IEnumerable<Worklog> Worklogs
    {
        get => _worklogs.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id);
        set => SetValue(ref _worklogs, value);
    }

    public async Task InitAsync(DateOnly? dateFilter, string? search)
    {
        if (dateFilter is not null)
        {
            SelectedDate = dateFilter.Value;
        }

        if (search is not null)
        {
            Search = search;
        }

        await LoadWorklogsAsync();
    }

    public async Task LoadWorklogsAsync()
    {
        LoadWorklogs = true;

        try
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Worklogs = [.. (await _dataService.GetWorklogsAsync(0, 0, $@"Description.Contains(""{Search}"")")).Items];
            }
            else
            {
                Worklogs = [.. (await _dataService.GetWorklogsAsync(0, 0, $@"Date == ""{_selectedDate:yyyy-MM-dd}""")).Items];
            }
        }
        catch
        {
            _popupService.Error("Fehler beim Laden der Arbeitsaufwände!");
        }
        finally
        {
            LoadWorklogs = false;
        }
    }

    public async Task<bool> SaveWorklogAsync(Worklog? worklog = null)
    {
        if (worklog is null)
        {
            try
            {
                var defaultTicket = (await _dataService.GetTicketsAsync(1, 0)).Items.FirstOrDefault();

                if (defaultTicket is null)
                {
                    _popupService.Error("Es gibt noch kein Ticket!");
                    return false;
                }

                worklog = new Worklog
                {
                    Date = DateOnly.FromDateTime(DateTimeOffset.Now.Date),
                    TicketId = defaultTicket.Id,
                    TicketTitle = defaultTicket.Title,
                    TimeSpent = TimeSpan.Zero,
                };
            }
            catch
            {
                _popupService.Error("Fehler beim Speichern vom Arbeitsaufwand!");
                return false;
            }
        }

        Worklog savedWorklog;

        try
        {
            savedWorklog = await _dataService.SaveWorklogAsync(worklog);
        }
        catch
        {
            _popupService.Error("Fehler beim Speichern vom Arbeitsaufwand!");
            return false;
        }

        Worklogs = [.. Worklogs.Where(x => x.Id != worklog.Id).Append(savedWorklog)];

        return true;
    }

    public async Task<bool> DeleteWorklogAsync(Worklog worklog)
    {
        if (!(await _popupService.Confim("Arbeitsaufwand löschen", "Möchtest du den Arbeitsaufwand wirklich löschen?")))
        {
            return false;
        }

        try
        {
            await _dataService.DeleteWorklogAsync(worklog.Id);
        }
        catch
        {
            _popupService.Error("Fehler beim Löschen vom Worklog!");
            return false;
        }

        Worklogs = [.. Worklogs.Where(x => x.Id != worklog.Id)];

        return true;
    }
}
