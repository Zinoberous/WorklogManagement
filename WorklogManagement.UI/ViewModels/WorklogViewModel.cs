using WorklogManagement.Service;

namespace WorklogManagement.UI.ViewModels;

public class WorklogViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
}
