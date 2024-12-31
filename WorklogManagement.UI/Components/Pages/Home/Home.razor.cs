using WorklogManagement.UI.Components.Pages.Base;

namespace WorklogManagement.UI.Components.Pages.Home;

public partial class Home : BasePage<HomeViewModel>
{
    private static string CalendarStyle => "max-width: calc(100vw - var(--sidebar-width) - 2 * var(--rz-row-gap) - 2 * var(--rz-panel-padding) - var(--scrollbar-width));";

    private IEnumerable<HomeCalendarDataRow> CalendarData
    {
        get
        {
            ICollection<HomeCalendarDataRow> result = [];

            DateOnly firstMonth = new(ViewModel.LoadDataFrom.Year, ViewModel.LoadDataFrom.Month, 1);
            DateOnly lastMonth = new(ViewModel.LoadDataTo.Year, ViewModel.LoadDataTo.Month, 1);

            for (var currentMonth = firstMonth; currentMonth <= lastMonth; currentMonth = currentMonth.AddMonths(1))
            {
                ICollection<HomeCalendarDataCell> days = [];

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
}
