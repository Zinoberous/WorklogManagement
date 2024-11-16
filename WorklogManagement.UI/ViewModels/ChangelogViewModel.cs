using WorklogManagement.Service;

namespace WorklogManagement.UI.ViewModels;

public class ChangelogViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
}
