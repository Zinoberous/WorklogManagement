using Microsoft.JSInterop;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.TicketList;

public class TicketListViewModel(IDataService dataService, INavigationService navigationService, IJSRuntime JSRuntime, IPopupService popupService) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IJSRuntime _jSRuntime = JSRuntime;
    private readonly IPopupService _popupService = popupService;

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
        await _jSRuntime.InvokeVoidAsync("localStorage.setItem", "ticket-list.status", string.Join(",", StatusFilter));
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

    public async Task OnSearchChanged()
    {
        _navigationService.UpdateQuery("search", Search);
        await LoadPageAsync();
    }

    private uint _pageSize = 50;
    public uint PageSize
    {
        get => _pageSize;
        set => SetValue(ref _pageSize, value);
    }

    public async Task OnPageSizeChanged() => await LoadPageAsync();

    private uint _pageIndex = 0;
    public uint PageIndex
    {
        get => _pageIndex;
        set => SetValue(ref _pageIndex, value);
    }

    public async Task OnPageIndexChange() => await LoadPageAsync();

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

    public async Task InitAsync(IEnumerable<TicketStatus>? statusFilter = null, string? search = null)
    {
        if (statusFilter is not null)
        {
            StatusFilter = statusFilter.ToList();
        }

        if (search is not null)
        {
            Search = search;
        }

        await LoadPageAsync();
    }

    public async Task LoadPageAsync()
    {
        LoadPage = true;

        try
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Page = await _dataService.GetTicketsPageBySearchAsync(PageSize, PageIndex, Search);
            }
            else
            {
                Page = await _dataService.GetTicketsPageByStatusFilterAsync(PageSize, PageIndex, StatusFilter.Select(x => (TicketStatus)x));
            }
        }
        catch (Exception ex)
        {
            await _popupService.Error("Fehler beim Laden der Tickets!", ex);
        }
        finally
        {
            LoadPage = false;
        }
    }

    public async Task<bool> SaveTicketAsync(Ticket ticket)
    {
        Ticket savedTicket;

        try
        {
            savedTicket = await _dataService.SaveTicketAsync(ticket);
        }
        catch (Exception ex)
        {
            await _popupService.Error("Fehler beim Speichern vom Ticket!", ex);
            return false;
        }

        // nur wenn neuer Eintrag
        if (savedTicket.Id != ticket.Id)
        {
            _popupService.Success($"Tichet {savedTicket.Id} wurde gespeichert");
        }

        await LoadPageAsync();

        return true;
    }

    public async Task<bool> DeleteTicketAsync(Ticket ticket)
    {
        if (!(await _popupService.Confim("Ticket löschen", "Möchtest du das Ticket wirklich löschen?")))
        {
            return false;
        }

        try
        {
            await _dataService.DeleteTicketAsync(ticket.Id);
        }
        catch (Exception ex)
        {
            await _popupService.Error("Fehler beim Löschen vom Ticket!", ex);
            return false;
        }

        await LoadPageAsync();

        _popupService.Info($"Ticket {ticket.Id} wurde gelöscht!");

        return true;
    }
}
