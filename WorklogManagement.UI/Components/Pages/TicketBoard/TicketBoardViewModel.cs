using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.TicketBoard;

public class TicketBoardViewModel(IDataService dataService, TimeProvider timeProvider, IPopupService popupService, INavigationService navigationService) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IPopupService _popupService = popupService;
    private readonly INavigationService _navigationService = navigationService;

    private bool _isOpenNewDialog = false;
    public bool IsOpenNewDialog
    {
        get => _isOpenNewDialog;
        set => SetValue(ref _isOpenNewDialog, value);
    }

    public void OpenNewDialog() => IsOpenNewDialog = true;

    private string _search = string.Empty;
    public string Search
    {
        get => _search;
        set
        {
            if (SetValue(ref _search, value))
            {
                OnSearchChanged();
            }
        }
    }

    private void OnSearchChanged()
    {
        _navigationService.UpdateQuery("search", Search);
    }

    private bool _isLoading = true;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetValue(ref _isLoading, value);
    }

    private IEnumerable<Ticket> _allTickets = [];
    private IEnumerable<Ticket> AllTickets
    {
        get => _allTickets;
        set => SetValue(ref _allTickets, value);
    }

    public IEnumerable<Ticket> Tickets => AllTickets.Where(x => string.IsNullOrWhiteSpace(Search) || x.Title.Contains(Search, StringComparison.OrdinalIgnoreCase));

    public async Task InitAsync(string? search)
    {
        if (search is not null)
        {
            Search = search;
        }

        await LoadTicketsAsync();
    }

    public async Task LoadTicketsAsync(bool silent = false)
    {
        if (!silent)
        {
            IsLoading = true;
        }

        try
        {
            IEnumerable<TicketStatus> statusFilter = [
                TicketStatus.Todo,
                TicketStatus.Running,
                TicketStatus.Paused,
                TicketStatus.Blocked,
                TicketStatus.Continuous
            ];

            AllTickets = (await _dataService.GetTicketsAsync(0, 0, $"""status in ({string.Join(',', statusFilter.Select(x => (int)x))}) OR TicketStatusLogs.Any(StartedAt >= "{_timeProvider.GetLocalNow().AddDays(-5):yyyy-MM-dd}")""")).Items;
        }
        catch
        {
            _popupService.Error("Fehler beim Laden der Tickets!");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<bool> SaveTicketAsync(Ticket ticket)
    {
        AllTickets = AllTickets.Any(x => x.Id == ticket.Id)
            ? AllTickets.Select(x => x.Id == ticket.Id ? ticket : x).ToArray()
            : [.. AllTickets, ticket];

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

        await LoadTicketsAsync(true);

        return true;
    }

    public async Task<bool> DeleteTicketAsync(Ticket ticket)
    {
        if (!(await _popupService.Confirm("Ticket löschen", "Möchtest du das Ticket wirklich löschen?")))
        {
            return false;
        }

        AllTickets = [.. AllTickets.Where(x => x.Id != ticket.Id)];

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

        await LoadTicketsAsync(true);

        return true;
    }
}
