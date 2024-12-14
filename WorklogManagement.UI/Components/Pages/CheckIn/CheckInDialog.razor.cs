using Microsoft.AspNetCore.Components;
using Radzen;
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

    private string? _selectedType;
    private string? SelectedType
    {
        get => _selectedType ?? TypeOptions.FirstOrDefault();
        set => _selectedType = value;
    }

    private string @TypeStyle => $"color: black; background-color: {Constant.CalendarEntryColor[SelectedType ?? string.Empty]}";

    private void TypeRender(DropDownItemRenderEventArgs<string?> args)
    {
        var type = args.Item.ToString();

        if (type == SelectedType)
        {
            args.Attributes.Add("style", "display: none;");
        }
        else if (!string.IsNullOrWhiteSpace(type))
        {
            args.Attributes.Add("style", $"color: black; background-color: {Constant.CalendarEntryColor[type]};");
        }
    }

    [Parameter]
    public EventCallback<WorkTime> OnSaveWorkTime { get; set; }

    [Parameter]
    public EventCallback<Absence> OnSaveAbsence { get; set; }

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
