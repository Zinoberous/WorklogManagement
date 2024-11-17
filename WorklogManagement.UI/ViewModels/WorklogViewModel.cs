using Microsoft.AspNetCore.Components;
using WorklogManagement.Service;

namespace WorklogManagement.UI.ViewModels;

public class WorklogViewModel(NavigationManager navigationManager, IWorklogManagementService service) : BaseViewModel(navigationManager)
{
    private readonly IWorklogManagementService _service = service;
}
