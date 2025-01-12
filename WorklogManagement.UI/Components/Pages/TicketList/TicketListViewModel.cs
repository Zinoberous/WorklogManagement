using Blazored.LocalStorage;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.TicketList;

public class TicketListViewModel(IDataService dataService, IPopupService popupService, ILocalStorageService localStorageService, INavigationService navigationService) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly IPopupService _popupService = popupService;
    private readonly ILocalStorageService _localStorageService = localStorageService;
    private readonly INavigationService _navigationService = navigationService;

    private bool _isOpenNewDialog = false;
    public bool IsOpenNewDialog
    {
        get => _isOpenNewDialog;
        set => SetValue(ref _isOpenNewDialog, value);
    }

    public void OpenNewDialog() => IsOpenNewDialog = true;

    public IReadOnlyDictionary<TicketStatus, string> StatusFilterOptions => Enum.GetValues<TicketStatus>().ToDictionary(x => x, x => x.ToString());

    private IList<TicketStatus> _statusFilter = [
        TicketStatus.Todo,
        TicketStatus.Running,
        TicketStatus.Paused,
        TicketStatus.Blocked,
        //TicketStatus.Done,
        //TicketStatus.Canceled,
        TicketStatus.Continuous,
    ];

    public IList<TicketStatus> StatusFilter
    {
        get => _statusFilter;
        set
        {
            if (SetValue(ref _statusFilter, [.. value.Order()]))
            {
                _ = OnSelectedStatusFilterChanged();
            }
        }
    }

    public async Task OnSelectedStatusFilterChanged()
    {
        _navigationService.UpdateQuery("status", string.Join(",", StatusFilter));
        await _localStorageService.SetItemAsync("ticket-list.status", string.Join(",", StatusFilter));
        await LoadPageAsync();
    }

    private string _search = string.Empty;
    public string Search
    {
        get => _search;
        set
        {
            if (SetValue(ref _search, value))
            {
                _ = OnSearchChanged();
            }
        }
    }

    private async Task OnSearchChanged()
    {
        _navigationService.UpdateQuery("search", Search);
        await LoadPageAsync();
    }

    private int _pageSize = 50;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetValue(ref _pageSize, value))
            {
                _ = LoadPageAsync();
            }
        }
    }

    private int _pageIndex = 0;
    public int PageIndex
    {
        get => _pageIndex;
        set
        {
            if (SetValue(ref _pageIndex, value))
            {
                _ = LoadPageAsync();
            }
        }
    }

    private bool _loadPage = true;
    public bool LoadPage
    {
        get => _loadPage;
        set => SetValue(ref _loadPage, value);
    }

    private Page<Ticket> _page = Page<Ticket>.Empty;
    public Page<Ticket> Page
    {
        get => _page;
        set => SetValue(ref _page, value);
    }

    public async Task InitAsync(string? statusFilter, string? search)
    {
        statusFilter ??= await _localStorageService.GetItemAsync<string>("ticket-list.status");

        if (statusFilter is not null)
        {
            StatusFilter = statusFilter.Split(',').Select(Enum.Parse<TicketStatus>).ToList();
        }

        if (search is not null)
        {
            Search = search;
        }

        await LoadPageAsync();
    }

    public async Task LoadPageAsync(bool silent = false)
    {
        if (!silent)
        {
            LoadPage = true;
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Page = await _dataService.GetTicketsAsync(PageSize, PageIndex, $@"Title.Contains(""{Search}"") || Description.Contains(""{Search}"")");
            }
            else
            {
                Page = await _dataService.GetTicketsAsync(PageSize, PageIndex, $"status in ({string.Join(',', StatusFilter.Select(x => (int)x))})");
            }
        }
        catch
        {
            _popupService.Error("Fehler beim Laden der Tickets!");
        }
        finally
        {
            LoadPage = false;
        }
    }

    public async Task<bool> SaveTicketAsync(Ticket ticket)
    {
        Page = Page with { Items = Page.Items.Select(x => x.Id == ticket.Id ? ticket : x).ToArray() };

        Ticket savedTicket;

        try
        {
            savedTicket = await _dataService.SaveTicketAsync(ticket);
        }
        catch
        {
            _popupService.Error("Fehler beim Speichern vom Ticket!");
            return false;
        }

        // nur wenn neuer Eintrag
        if (savedTicket.Id != ticket.Id)
        {
            _popupService.Success($"Tichet {savedTicket.Id} wurde gespeichert");
        }

        await LoadPageAsync(true);

        return true;
    }

    public async Task<bool> DeleteTicketAsync(Ticket ticket)
    {
        if (!(await _popupService.Confim("Ticket löschen", "Möchtest du das Ticket wirklich löschen?")))
        {
            return false;
        }

        Page = Page with { Items = Page.Items.Where(x => x.Id != ticket.Id).ToArray() };

        try
        {
            await _dataService.DeleteTicketAsync(ticket.Id);
        }
        catch
        {
            _popupService.Error("Fehler beim Löschen vom Ticket!");
            return false;
        }

        _popupService.Info($"Ticket {ticket.Id} wurde gelöscht!");

        await LoadPageAsync(true);

        return true;
    }
}
