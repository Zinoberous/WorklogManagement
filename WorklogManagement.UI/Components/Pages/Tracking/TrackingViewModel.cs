using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.Tracking;

public class TrackingViewModel(
    IDataService dataService,
    IPopupService popupService,
    INavigationService navigationService)
    : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly IPopupService _popupService = popupService;
    private readonly INavigationService _navigationService = navigationService;

    public DateOnly SelectedDate
    {
        get;
        set
        {
            if (SetValue(ref field, value))
            {
                _ = OnSelectedDateChanged();
            }
        }
    } = DateOnly.FromDateTime(DateTime.Today);

    public async Task OnSelectedDateChanged()
    {
        _navigationService.UpdateQuery("date", $"{SelectedDate:yyyy-MM-dd}");
        await LoadWorklogsAsync();
    }

    public string Search
    {
        get;
        set
        {
            if (SetValue(ref field, string.IsNullOrWhiteSpace(value) ? string.Empty : value))
            {
                _ = OnSearchChanged();
            }
        }
    } = string.Empty;

    private async Task OnSearchChanged()
    {
        _navigationService.UpdateQuery("search", Search);
        await LoadWorklogsAsync();
    }

    public bool LoadWorklogs { get; set => SetValue(ref field, value); } = true;

    public IEnumerable<Worklog> Worklogs
    {
        get => field.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id);
        set => SetValue(ref field, value);
    } = [];

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
                Worklogs = [.. (await _dataService.GetWorklogsAsync(0, 0, $@"Date == ""{SelectedDate:yyyy-MM-dd}""")).Items];
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

                worklog = new()
                {
                    Date = SelectedDate,
                    Ticket = new()
                    {
                        Id = defaultTicket.Id,
                        Title = defaultTicket.Title,
                        Status = defaultTicket.Status,
                    },
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

        Worklogs = [.. Worklogs.Any(x => x.Id == savedWorklog.Id)
            ? Worklogs.Select(x => x.Id == savedWorklog.Id ? savedWorklog : x)
            : Worklogs.Append(savedWorklog)];

        return true;
    }

    public async Task<bool> DeleteWorklogAsync(Worklog worklog)
    {
        if (!(await _popupService.Confirm("Arbeitsaufwand löschen", "Möchtest du den Arbeitsaufwand wirklich löschen?")))
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
