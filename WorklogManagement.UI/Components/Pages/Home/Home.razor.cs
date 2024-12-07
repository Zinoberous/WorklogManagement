namespace WorklogManagement.UI.Components.Pages.Home;

public partial class Home
{
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
