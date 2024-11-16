using WorklogManagement.Data.Context;

namespace WorklogManagement.UI.ViewModels;

public class TicketViewModel(WorklogManagementContext context) : BaseViewModel
{
    private readonly WorklogManagementContext _context = context;
}
