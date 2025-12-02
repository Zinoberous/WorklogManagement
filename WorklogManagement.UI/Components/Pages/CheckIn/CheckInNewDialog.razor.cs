using Microsoft.AspNetCore.Components;
using Radzen;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Common;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public partial class CheckInNewDialog
{
    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter]
    public IEnumerable<string> UsedTypeOptions
    {
        get;
        set
        {
            field = value;
            SelectedType = DefaultSelectedType;
        }
    } = [];

    private IEnumerable<string> TypeOptions => [
        .. Constant.WorkTimeLabels.Values.Concat(Constant.AbsenceLabels.Values)
            .Where(value => !UsedTypeOptions.Contains(value))];

    private string DefaultSelectedType => TypeOptions.First();

    private string SelectedType { get => field ?? DefaultSelectedType; set => field = value; }

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
        => await Save(false);

    private async Task Save(bool close)
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

        if (close)
        {
            await Close();
        }
        else
        {
            Reset();
        }
    }

    private async Task Close()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(IsOpen);

        Reset();
    }

    private async Task SaveAndClose()
        => await Save(true);

    private void Reset()
    {
        SelectedType = DefaultSelectedType;
        Actual = TimeSpan.FromHours(8);
        Expected = TimeSpan.FromHours(8);
        Note = null;
    }
}
