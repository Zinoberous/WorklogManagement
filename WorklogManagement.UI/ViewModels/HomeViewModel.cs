using WorklogManagement.Service;
using WorklogManagement.Service.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.ViewModels;

public class HomeViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;

    public ObservableProperty<bool> LoadOvertime { get; } = new(true);

    public ObservableProperty<OvertimeInfo?> Overtime { get; } = new();

    public async Task LoadOvertimeAsync()
    {
        Overtime.Value = await _service.GetOvertimeAsync();

        LoadOvertime.Value = false;
    }
}
