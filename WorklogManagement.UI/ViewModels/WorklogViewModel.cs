using WorklogManagement.Data;
using WorklogManagement.Data.Context;

namespace WorklogManagement.UI.ViewModels;

public class WorklogViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;
}
