using WorklogManagement.UI.Components.Pages.Base;

namespace WorklogManagement.UI.Components.Pages.TicketList;

public class TicketListViewModel : BaseViewModel
{
    private bool _loadTickets = true;
    public bool LoadTickets
    {
        get => _loadTickets;
        set => SetValue(ref _loadTickets, value);
    }
}
