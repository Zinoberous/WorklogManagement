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
    public IEnumerable<string> UsedTypeOptions { get; set; } = [];

    private IEnumerable<string> TypeOptions =>
        Constant.WorkTimeLabels.Values
        .Concat(Constant.AbsenceLabels.Values)
        .Where(value => !UsedTypeOptions.Contains(value))
        .ToArray();

    private string? _selectedType;
    private string SelectedType
    {
        get => _selectedType ?? TypeOptions.First();
        set => _selectedType = value;
    }

    private string @TypeStyle => $"color: black; background-color: {Constant.CalendarEntryColor[SelectedType ?? string.Empty]}";

    private void TypeRender(DropDownItemRenderEventArgs<string> args)
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
    public DateOnly Date { get; set; }

    [Parameter]
    public EventCallback<DateOnly> DateChanged { get; set; }

    private TimeSpan Actual { get; set; } = TimeSpan.FromHours(8);

    private TimeSpan Expected { get; set; } = TimeSpan.FromHours(8);

    private string? Note { get; set; }

    [Parameter]
    public Func<WorkTime, Task<bool>> OnSaveWorkTime { get; set; } = _ => Task.FromResult(false);

    [Parameter]
    public Func<Absence, Task<bool>> OnSaveAbsence { get; set; } = _ => Task.FromResult(false);

    private async Task Save()
    {
        var saved = false;

        if (Constant.WorkTimeLabels.ContainsValue(SelectedType))
        {
            WorkTime workTime = new()
            {
                Type = Constant.WorkTimeLabels.First(x => x.Value == SelectedType).Key,
                Date = Date,
                Actual = Actual,
                Expected = Expected,
                Note = string.IsNullOrWhiteSpace(Note)
                    ? null
                    : Note
            };

            saved = await OnSaveWorkTime.Invoke(workTime);
        }
        else // Constant.AbsenceLabels.ContainsValue(SelectedType)
        {
            Absence absence = new()
            {
                Type = Constant.AbsenceLabels.First(x => x.Value == SelectedType).Key,
                Date = Date,
                Duration = Actual,
                Note = string.IsNullOrWhiteSpace(Note)
                    ? null
                    : Note
            };

            saved = await OnSaveAbsence.Invoke(absence);
        }

        if (!saved)
        {
            return;
        }

        UsedTypeOptions = UsedTypeOptions.Append(SelectedType).ToArray();

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
