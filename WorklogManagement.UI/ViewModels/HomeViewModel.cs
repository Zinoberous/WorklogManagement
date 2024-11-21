using WorklogManagement.Service;
using WorklogManagement.Service.Models;

namespace WorklogManagement.UI.ViewModels;

public class HomeViewModel(IHttpClientFactory httpClientFactory, IWorklogManagementService service) : BaseViewModel
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IWorklogManagementService _service = service;

    //public string[] DayLabelShorts { get; } = ["So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"];
    //public string[] MonthLabelShorts { get; } = ["Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez"];

    private bool _loadOvertime = true;
    public bool LoadOvertime
    {
        get => _loadOvertime;
        set => SetValue(ref _loadOvertime, value);
    }

    private Exception? _loadOvertimeError;
    public Exception? LoadOvertimeError
    {
        get => _loadOvertimeError;
        set => SetValue(ref _loadOvertimeError, value);
    }

    private OvertimeInfo? _overtime;
    public OvertimeInfo? Overtime
    {
        get => _overtime;
        set => SetValue(ref _overtime, value);
    }

    private int _selectedYear = DateTimeOffset.Now.Year;
    public int SelectedYear
    {
        get => _selectedYear;
        set => SetValue(ref _selectedYear, value);
    }

    //public ObservableProperty<string> SelectedFederalState { get; }
    //public ObservableProperty<bool> LoadYear { get; }
    //public ObservableProperty<Exception?> LoadYearError { get; }
    //public ObservableProperty<Dictionary<DateOnly, string>> Holidays { get; }
    //public ObservableProperty<IEnumerable<WorkTime>> WorkTimes { get; }
    //public ObservableProperty<IEnumerable<Absence>> Absences { get; }

    public async Task LoadOvertimeAsync()
    {
        LoadOvertime = true;

        try
        {
            Overtime = await _service.GetOvertimeAsync();
        }
        catch (Exception ex)
        {
            LoadOvertimeError = ex;
        }
        finally
        {
            LoadOvertime = false;
        }
    }

    //public string GetDayLable(int month, int day)
    //{
    //    try
    //    {
    //        DateOnly date = new(SelectedYear.Value, month, day);

    //        return DayLabelShorts[(int)date.DayOfWeek];
    //    }
    //    catch
    //    {
    //        return string.Empty;
    //    }
    //}

    //public async Task LoadHolydaysAsync()
    //{
    //    using var client = _httpClientFactory.CreateClient();

    //    var res = await client.GetAsync($"https://date.nager.at/api/v3/PublicHolidays/{SelectedYear.Value}/DE");
    //}

    //private async Task OnSelectedYearChangedAsync()
    //{

    //}

    //private async Task OnSelectedFederalStateChangedAsync()
    //{

    //}
}
