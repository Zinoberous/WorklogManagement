using WorklogManagement.Shared.Enums;

namespace WorklogManagement.UI.Components.Pages.Home;

public partial class Home
{
    private static readonly Dictionary<WorkTimeType, string> _workTimeLabels = new()
    {
        { WorkTimeType.Office, "Büro" },
        { WorkTimeType.Mobile, "Mobil" },
    };

    private static readonly Dictionary<string, string> _federalStates = new()
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

    protected override void OnInitialized()
    {
        _ = Task.WhenAll([
            ViewModel.LoadOvertimeAsync(),
            ViewModel.LoadCalendarStatisticsAsync(),
            ViewModel.LoadTicketStatisticsAsync(),
            ViewModel.LoadWorkTimesAsync(),
            ViewModel.LoadAbsencesAsync(),
            ViewModel.LoadHolidaysAsync(),
        ]);
    }
}
