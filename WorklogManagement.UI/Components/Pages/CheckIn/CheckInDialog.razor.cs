using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Common;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public partial class CheckInDialog
{
    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter]
    public DateOnly Date { get; set; }

    [Parameter]
    public EventCallback<DateOnly> DateChanged { get; set; }

    [Parameter]
    public IEnumerable<string> TypeOptions { get; set; } = [];

    [Parameter]
    public EventCallback<WorkTime> OnSaveWorkTime { get; set; }

    [Parameter]
    public EventCallback<Absence> OnSaveAbsence { get; set; }

    private string? _selectedType;
    private string? SelectedType
    {
        get => _selectedType ?? TypeOptions.FirstOrDefault();
        set => _selectedType = value;
    }

    private TimeSpan Actual { get; set; } = TimeSpan.FromHours(8);
    private TimeSpan Expected { get; set; } = TimeSpan.FromHours(8);

    private async Task Save()
    {
        // TODO: if SelectedType is WorkTime then call OnSaveWorkTime
        // TODO: if SelectedType is Absence then call OnSaveAbsence
        await Task.CompletedTask;

        TypeOptions = TypeOptions.Where(x => x != SelectedType).ToArray();

        _selectedType = null;
    }

    private async Task Close()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(IsOpen);
    }

    private async Task SaveAndClose()
    {
        await Save();
        await Close();
    }
}
