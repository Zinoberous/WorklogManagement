using Microsoft.AspNetCore.Components;
using WorklogManagement.Service;
using WorklogManagement.Service.Enums;
using WorklogManagement.Service.Models;
using WorklogManagement.Service.Models.Queries;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.ViewModels;

public class TrackingViewModel : BaseViewModel
{
    public ObservableProperty<DateOnly> Date { get; }
    public ObservableProperty<string> Search { get; }
    public ObservableProperty<IEnumerable<Ticket>> RunningTickets { get; }
    public ObservableProperty<IEnumerable<Worklog>> Worklogs { get; }
    public ObservableProperty<IEnumerable<int>> VisibleWorklogNotes { get; }
    public ObservableProperty<(bool IsOpen, bool Load, int WorklogId, IEnumerable<WorklogAttachment> Attachments)> AttachmentDialog { get; }

    private readonly IWorklogManagementService _service;

    public TrackingViewModel(NavigationManager navigationManager, IWorklogManagementService service) : base(navigationManager)
    {
        _service = service;

        Date = new(DateOnly.FromDateTime(DateTime.Today), OnDateChangedAsync);
        Search = new(string.Empty, OnSearchChangedAsync);
        RunningTickets = new();
        Worklogs = new();
        VisibleWorklogNotes = new();
        AttachmentDialog = new();
    }

    public async Task InitAsync(DateOnly? initialDate = null, string? initialSearch = null)
    {
        await TryLoadAsync(async () =>
        {
            if (initialDate.HasValue)
            {
                Date.SetValueNoChangeEvent(initialDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(initialSearch))
            {
                Search.SetValueNoChangeEvent(initialSearch);
            }

            await GetWorklogsAsync();

            TicketQuery query = new()
            {
                Status = [TicketStatus.Running, TicketStatus.Continuous]
            };

            var page = await _service.GetTicketsAsync(query);

            RunningTickets.Value = page.Items;
        });
    }

    public async Task LoadWorklogsAsync()
    {
        await TryLoadAsync(GetWorklogsAsync);
    }

    private async Task GetWorklogsAsync()
    {
        WorklogQuery query = string.IsNullOrWhiteSpace(Search.Value)
            ? new() { Date = Date.Value }
            : new() { Search = Search.Value };

        var page = await _service.GetWorklogsAsync(query);

        Worklogs.Value = page.Items;
    }

    private async Task OnDateChangedAsync()
    {
        if (!string.IsNullOrWhiteSpace(Search.Value))
        {
            return;
        }

        UpdateQuery("date", $"{Date!.Value:yyyy-MM-dd}");

        await LoadWorklogsAsync();
    }

    private async Task OnSearchChangedAsync()
    {
        UpdateQuery("search", Search!.Value);

        await LoadWorklogsAsync();
    }
}
