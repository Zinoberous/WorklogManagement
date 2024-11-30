//using WorklogManagement.Shared.Models;
//using WorklogManagement.UI.Models;

//namespace WorklogManagement.UI.ViewModels;

//public class CheckInViewModel(IHttpClientFactory httpClientFactory, INavigator navigator, INotifier notifier) : BaseViewModel
//{
//    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
//    private readonly INavigator _navigator = navigator;
//    private readonly INotifier _notifier = notifier;

//    private DateOnly _date = DateOnly.FromDateTime(DateTime.Today);
//    public DateOnly Date
//    {
//        get => _date;
//        set
//        {
//            if (SetValue(ref _date, value))
//            {
//                _ = OnDateChangedAsync();
//            }
//        }
//    }

//    private bool _isLoading = true;
//    public bool IsLoading
//    {
//        get => _isLoading;
//        set => SetValue(ref _isLoading, value);
//    }

//    private IEnumerable<WorkTime> _workTimes = [];
//    public IEnumerable<WorkTime> WorkTimes
//    {
//        get => _workTimes;
//        set => SetValue(ref _workTimes, value);
//    }

//    private IEnumerable<Absence> _absences = [];
//    public IEnumerable<Absence> Absences
//    {
//        get => _absences;
//        set => SetValue(ref _absences, value);
//    }

//    public async Task InitAsync(DateOnly? initialDate = null)
//    {
//        if (initialDate.HasValue)
//        {
//            Date = initialDate.Value;
//        }

//        await LoadWorkTimesAndAbsencesAsync();
//    }

//    public async Task LoadWorkTimesAndAbsencesAsync()
//    {
//        IsLoading = true;

//        try
//        {
//            WorkTimes = await _service.GetWorkTimesAsync(Date);
//            Absences = await _service.GetAbsencesAsync(Date);
//        }
//        catch (Exception ex)
//        {
//            await _notifier.NotifyErrorAsync("Fehler beim Laden der Einträge!", ex);
//        }
//        finally
//        {
//            IsLoading = false;
//        }
//    }

//    private async Task OnDateChangedAsync()
//    {
//        _navigator.UpdateQuery("date", $"{Date:yyyy-MM-dd}");

//        await LoadWorkTimesAndAbsencesAsync();
//    }
//}
