using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WorklogManagement.UI.Components.Pages.Home;

public partial class Home : IAsyncDisposable
{
    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    //private DotNetObjectReference<Home>? _dotNetRef;

    // OnAfterRenderAsync verhindert das routing, bis Daten geladen sind, daher wurde es, bis einer neue Lösung gefunden wird, erstmal zurückgesestellt
    private string _calendarStyle = "max-width: calc(100vw - var(--sidebar-width) - 2 * var(--rz-row-gap) - 2 * var(--rz-panel-padding) - 16px);";
    private string CalendarStyle
    {
        get => _calendarStyle;
        set
        {
            if (_calendarStyle != value)
            {
                _calendarStyle = value;
                StateHasChanged();
            }
        }
    }

    private IEnumerable<HomeCalendarDataRow> CalendarData
    {
        get
        {
            List<HomeCalendarDataRow> result = [];

            DateOnly firstMonth = new(ViewModel.LoadDataFrom.Year, ViewModel.LoadDataFrom.Month, 1);
            DateOnly lastMonth = new(ViewModel.LoadDataTo.Year, ViewModel.LoadDataTo.Month, 1);

            for (var currentMonth = firstMonth; currentMonth <= lastMonth; currentMonth = currentMonth.AddMonths(1))
            {
                List<HomeCalendarDataCell> days = [];

                DateOnly firstDate = new(currentMonth.Year, currentMonth.Month, 1);
                DateOnly lastDate = new(currentMonth.Year, currentMonth.Month, DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month));

                for (var currentDate = firstDate; currentDate <= lastDate; currentDate = currentDate.AddDays(1))
                {
                    days.Add(new()
                    {
                        Date = currentDate,
                        WorkTimes = ViewModel.WorkTimes.Where(x => x.Date == currentDate).ToArray(),
                        Absences = ViewModel.Absences.Where(x => x.Date == currentDate).ToArray(),
                        Holiday = ViewModel.Holidays.FirstOrDefault(x => x.Date == currentDate),
                    });
                }

                result.Add(new()
                {
                    Year = currentMonth.Year,
                    Month = currentMonth.Month,
                    Days = days
                });
            }

            return result;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.WhenAll([
            ViewModel.LoadOvertimeAsync(),
            ViewModel.LoadCalendarStatisticsAsync(),
            ViewModel.LoadTicketStatisticsAsync(),
            ViewModel.LoadWorkTimesAsync(),
            ViewModel.LoadAbsencesAsync(),
            ViewModel.LoadHolidaysAsync(),
        ]);
    }

    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    if (firstRender)
    //    {
    //        UpdateScrollbarWidth(await GetScrollbarWidthAsync());

    //        _dotNetRef = DotNetObjectReference.Create(this);
    //        await JSRuntime.InvokeVoidAsync("setResizeCallback", _dotNetRef);
    //    }
    //}

    //[JSInvokable]
    //public void UpdateScrollbarWidth(int scrollbarWidth)
    //{
    //    CalendarStyle = $"max-width: calc(100vw - var(--sidebar-width) - 2 * var(--rz-row-gap) - 2 * var(--rz-panel-padding) - {scrollbarWidth}px);";
    //}

    //private async Task<int> GetScrollbarWidthAsync()
    //{
    //    return await JSRuntime.InvokeAsync<int>("getRzBodyScrollbarWidth");
    //}

    #region dispose

    private bool _disposed = false;

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!_disposed && disposing)
        {
            await Task.CompletedTask;
            //if (_dotNetRef != null)
            //{
            //    await JSRuntime.InvokeVoidAsync("removeResizeCallback");
            //    _dotNetRef.Dispose();
            //}

            _disposed = true;
        }
    }

    #endregion
}
