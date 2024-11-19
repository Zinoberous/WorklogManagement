using Microsoft.AspNetCore.Components;
using WorklogManagement.Service;
using WorklogManagement.Service.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWorklogManagementService _service;

    public string[] DayLabelShorts { get; } = ["So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"];
    public string[] MonthLabelShorts { get; } = ["Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez"];

    public ObservableProperty<bool> LoadOvertime { get; }
    public ObservableProperty<Exception?> LoadOvertimeError { get; }
    public ObservableProperty<OvertimeInfo?> Overtime { get; }

    public ObservableProperty<int> SelectedYear { get; }
    public ObservableProperty<string> SelectedFederalState { get; }
    public ObservableProperty<bool> LoadYear { get; }
    public ObservableProperty<Exception?> LoadYearError { get; }
    public ObservableProperty<Dictionary<DateOnly, string>> Holidays { get; }
    public ObservableProperty<IEnumerable<WorkTime>> WorkTimes { get; }
    public ObservableProperty<IEnumerable<Absence>> Absences { get; }

    public HomeViewModel(NavigationManager navigationManager, IHttpClientFactory httpClientFactory, IWorklogManagementService service) : base(navigationManager)
    {
        _httpClientFactory = httpClientFactory;
        _service = service;

        LoadOvertime = new(true);
        LoadOvertimeError = new(null);
        Overtime = new(null);
        SelectedYear = new(DateTimeOffset.Now.Year, OnSelectedYearChangedAsync);
        SelectedFederalState = new("DE_HE", OnSelectedFederalStateChangedAsync);
        LoadYear = new(true);
        LoadYearError = new(null);
        Holidays = new([]);
        WorkTimes = new([]);
        Absences = new([]);
    }

    public async Task LoadOvertimeAsync()
    {
        await TryLoadAsync(LoadOvertime, LoadOvertimeError, async () => Overtime.Value = await _service.GetOvertimeAsync());
    }

    public string GetDayLable(int month, int day)
    {
        try
        {
            DateOnly date = new(SelectedYear.Value, month, day);

            return DayLabelShorts[(int)date.DayOfWeek];
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task LoadHolydaysAsync()
    {
        using var client = _httpClientFactory.CreateClient();

        var res = await client.GetAsync($"https://date.nager.at/api/v3/PublicHolidays/{SelectedYear.Value}/DE");
    }

    private async Task OnSelectedYearChangedAsync()
    {

    }

    private async Task OnSelectedFederalStateChangedAsync()
    {

    }
}
