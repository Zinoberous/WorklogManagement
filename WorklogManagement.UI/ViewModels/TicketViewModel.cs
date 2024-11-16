using WorklogManagement.Data;

namespace WorklogManagement.UI.ViewModels;

public class TicketViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
}
