using WorklogManagement.Shared.Enums;

namespace WorklogManagement.UI.Common;

internal static class Constant
{
    internal static readonly Dictionary<WorkTimeType, string> WorkTimeLabels = new()
    {
        { WorkTimeType.Mobile, "Mobil" },
        { WorkTimeType.Office, "Büro" },
    };

    internal static readonly Dictionary<AbsenceType, string> AbsenceLabels = new()
    {
        { AbsenceType.Holiday, "Feiertag" },
        { AbsenceType.Vacation, "Urlaub" },
        { AbsenceType.Ill, "Krank" },
    };

    internal static readonly Dictionary<string, string> CalendarEntryColor = new()
    {
        { WorkTimeLabels[WorkTimeType.Mobile], "#8dd0ff" },
        { WorkTimeLabels[WorkTimeType.Office], "green" },
        { AbsenceLabels[AbsenceType.Holiday], "lightgreen" },
        { AbsenceLabels[AbsenceType.Vacation], "lightblue" },
        { AbsenceLabels[AbsenceType.Ill], "lightcoral" },
    };

    internal static readonly Dictionary<string, string> FederalStates = new()
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

    internal static readonly string[] MonthLabelShorts = ["Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez"];

    internal static readonly string[] DayLabelShorts = ["So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"];
}
