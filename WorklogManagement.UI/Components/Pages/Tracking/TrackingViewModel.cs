using WorklogManagement.UI.Components.Pages.Base;

namespace WorklogManagement.UI.Components.Pages.Tracking;

public class TrackingViewModel : BaseViewModel
{
    //    public ObservableProperty<bool> IsLoading { get; }
    //    public ObservableProperty<Exception?> LoadError { get; }
    //    public ObservableProperty<DateOnly> Date { get; }
    //    public ObservableProperty<string> Search { get; }
    //    public ObservableProperty<IEnumerable<Ticket>> RunningTickets { get; }
    //    public ObservableProperty<IEnumerable<Worklog>> Worklogs { get; }
    //    public ObservableProperty<IEnumerable<int>> VisibleWorklogNotes { get; }
    //    public ObservableProperty<(bool IsOpen, bool Load, int WorklogId, IEnumerable<WorklogAttachment> Attachments)> AttachmentDialog { get; }

    //    private readonly NavigationManager _navigationManager;
    //    private readonly IWorklogManagementService _service;

    //    public TrackingViewModel(NavigationManager navigationManager, IWorklogManagementService service)
    //    {
    //        _navigationManager = navigationManager;
    //        _service = service;

    //        IsLoading = new(true);
    //        LoadError = new();
    //        Date = new(DateOnly.FromDateTime(DateTime.Today), OnDateChangedAsync);
    //        Search = new(string.Empty, OnSearchChangedAsync);
    //        RunningTickets = new();
    //        Worklogs = new();
    //        VisibleWorklogNotes = new();
    //        AttachmentDialog = new();
    //    }

    //    public async Task InitAsync(DateOnly? initialDate = null, string? initialSearch = null)
    //    {
    //        await LoadContentHelper.TryLoadAsync(IsLoading, LoadError, async () =>
    //        {
    //            if (initialDate.HasValue)
    //            {
    //                Date.SetValueNoChangeEvent(initialDate.Value);
    //            }

    //            if (!string.IsNullOrWhiteSpace(initialSearch))
    //            {
    //                Search.SetValueNoChangeEvent(initialSearch);
    //            }

    //            await GetWorklogsAsync();

    //            TicketQuery query = new()
    //            {
    //                Status = [TicketStatus.Running, TicketStatus.Continuous]
    //            };

    //            var page = await _service.GetTicketsAsync(query);

    //            RunningTickets.Value = page.Items;
    //        });
    //    }

    //    public async Task LoadWorklogsAsync()
    //    {
    //        await LoadContentHelper.TryLoadAsync(IsLoading, LoadError, GetWorklogsAsync);
    //    }

    //    private async Task GetWorklogsAsync()
    //    {
    //        WorklogQuery query = string.IsNullOrWhiteSpace(Search.Value)
    //            ? new() { Date = Date.Value }
    //            : new() { Search = Search.Value };

    //        var page = await _service.GetWorklogsAsync(query);

    //        Worklogs.Value = page.Items;
    //    }

    //    private async Task OnDateChangedAsync()
    //    {
    //        if (!string.IsNullOrWhiteSpace(Search.Value))
    //        {
    //            return;
    //        }

    //        NavigationHelper.UpdateQuery(_navigationManager, "date", $"{Date!.Value:yyyy-MM-dd}");

    //        await LoadWorklogsAsync();
    //    }

    //    private async Task OnSearchChangedAsync()
    //    {
    //        NavigationHelper.UpdateQuery(_navigationManager, "search", Search!.Value);

    //        await LoadWorklogsAsync();
    //    }
}
