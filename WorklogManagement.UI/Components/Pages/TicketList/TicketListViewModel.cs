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

    public bool IsOpenNewDialog { get; set => SetValue(ref field, value); } = false;

    public void OpenNewDialog() => IsOpenNewDialog = true;

    public static IDictionary<TicketStatus, string> StatusFilterOptions => Enum.GetValues<TicketStatus>().ToDictionary(x => x, x => x.ToString());

    public IList<TicketStatus> StatusFilter
    {
        get;
        set
        {
            if (SetValue(ref field, [.. value.Order()]))
            {
                _ = OnSelectedStatusFilterChanged();
            }
        }
    } = [
        TicketStatus.Todo,
        TicketStatus.Running,
        TicketStatus.Paused,
        TicketStatus.Blocked,
        //TicketStatus.Done,
        //TicketStatus.Canceled,
        TicketStatus.Continuous,
    ];

    public async Task OnSelectedStatusFilterChanged()
    {
        _navigationService.UpdateQuery("status", string.Join(",", StatusFilter));
        await _localStorageService.SetItemAsync("ticket-list.status", string.Join(",", StatusFilter));
        await LoadPageAsync();
    }

    public string Search
    {
        get;
        set
        {
            if (SetValue(ref field, value))
            {
                _ = OnSearchChanged();
            }
        }
    } = string.Empty;

    private async Task OnSearchChanged()
    {
        _navigationService.UpdateQuery("search", Search);
        await LoadPageAsync();
    }

    public int PageSize
    {
        get;
        set
        {
            if (SetValue(ref field, value))
            {
                _ = LoadPageAsync();
            }
        }
    } = 50;

    public int PageIndex
    {
        get;
        set
        {
            if (SetValue(ref field, value))
            {
                _ = LoadPageAsync();
            }
        }
    } = 0;

    public bool LoadPage { get; set => SetValue(ref field, value); } = true;

    public Page<Ticket> Page { get; set => SetValue(ref field, value); } = Page<Ticket>.Empty;

    public async Task InitAsync(string? statusFilter, string? search)
    {
        statusFilter ??= await _localStorageService.GetItemAsync<string>("ticket-list.status");

        if (statusFilter is not null)
        {
            StatusFilter = [.. statusFilter.Split(',').Select(Enum.Parse<TicketStatus>)];
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
        Page = Page with { Items = [.. Page.Items.Select(x => x.Id == ticket.Id ? ticket : x)] };

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
        if (!(await _popupService.Confirm("Ticket löschen", "Möchtest du das Ticket wirklich löschen?")))
        {
            return false;
        }

        Page = Page with { Items = [.. Page.Items.Where(x => x.Id != ticket.Id)] };

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
