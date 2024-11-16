using WorklogManagement.Service;

namespace WorklogManagement.UI.ViewModels;

public class CheckInViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
}
