using Microsoft.AspNetCore.Components;
using WorklogManagement.Service;
using WorklogManagement.Service.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.ViewModels;

public class HomeViewModel(NavigationManager navigationManager, IWorklogManagementService service) : BaseViewModel(navigationManager)
{
    private readonly IWorklogManagementService _service = service;

    public ObservableProperty<OvertimeInfo?> Overtime { get; } = new();

    public async Task LoadOvertimeAsync()
    {
        await TryLoadAsync(async () =>
        {
            Overtime.Value = await _service.GetOvertimeAsync();
        });
    }
}
