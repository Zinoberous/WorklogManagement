using Microsoft.AspNetCore.Components;
using WorklogManagement.Service;
using WorklogManagement.Service.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.ViewModels;

public class HomeViewModel(NavigationManager navigationManager, IWorklogManagementService service) : BaseViewModel(navigationManager)
{
    private readonly IWorklogManagementService _service = service;

    public ObservableProperty<bool> LoadOvertime { get; } = new(true);
    public ObservableProperty<Exception?> LoadOvertimeError { get; } = new();
    public ObservableProperty<OvertimeInfo?> Overtime { get; } = new();

    public async Task LoadOvertimeAsync()
    {
        await TryLoadAsync(LoadOvertime, LoadOvertimeError, async () => Overtime.Value = await _service.GetOvertimeAsync());
    }
}
