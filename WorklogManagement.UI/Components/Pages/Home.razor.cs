namespace WorklogManagement.UI.Components.Pages;

public partial class Home
{
    private readonly Dictionary<string, string> federalStates = new()
    {
        { "DE-BW", "Baden-Württemberg" },
        { "DE-BY", "Bayern" },
        { "DE-BE", "Berlin" },
        { "DE-BB", "Brandenburg" },
        { "DE-HB", "Bremen" },
        { "DE-HH", "Hamburg" },
        { "DE-HE", "Hessen" },
        { "DE-MV", "Mecklenburg-Vorpommern" },
        { "DE-NI", "Niedersachsen" },
        { "DE-NW", "Nordrhein-Westfalen" },
        { "DE-RP", "Rheinland-Pfalz" },
        { "DE-SL", "Saarland" },
        { "DE-SN", "Sachsen" },
        { "DE-ST", "Sachsen-Anhalt" },
        { "DE-SH", "Schleswig-Holstein" },
        { "DE-TH", "Thüringen" }
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ViewModel.LoadOvertimeAsync();
            await ViewModel.LoadCalendarStatisticsAsync();
            await ViewModel.LoadTicketStatisticsAsync();
            await ViewModel.LoadWorkTimesAsync();
            await ViewModel.LoadAbsencesAsync();
            await ViewModel.LoadHolidaysAsync();
        }
    }
}
