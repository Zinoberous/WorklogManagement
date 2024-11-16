using WorklogManagement.Data;

namespace WorklogManagement.UI.ViewModels;

public class HomeViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
}
