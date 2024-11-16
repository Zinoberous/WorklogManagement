using WorklogManagement.Service;

namespace WorklogManagement.UI.ViewModels;

public class TrackingViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
}
