using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Components.Pages.Base;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.TicketForm;

public class TicketFormViewModel(IDataService dataService, INavigationService navigationService, IPopupService popupService) : BaseViewModel
{
    private readonly IDataService _dataService = dataService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IPopupService _popupService = popupService;

    private Ticket _ticket = null!;

    private bool _isLoading = true;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetValue(ref _isLoading, value);
    }

    public string Title
    {
        get => _ticket.Title;
        set => _ = SaveTicketAsync(_ticket with { Title = value });
    }

    public string? Description
    {
        get => _ticket.Description;
        set => _ = SaveTicketAsync(_ticket with { Description = value });
    }

    private bool _showDescription = true;
    public bool ShowDescription
    {
        get => _showDescription;
        set => SetValue(ref _showDescription, value);
    }

    public void ToggleDescription() => ShowDescription = !ShowDescription;

    public TicketStatus Status
    {
        get => _ticket.Status;
        set => _ = SaveTicketAsync(_ticket with { Status = value });
    }

    public string? StatusNote
    {
        get => _ticket.StatusNote;
        set => _ = SaveTicketAsync(_ticket with { StatusNote = value });
    }

    private bool _showStatusNote = false;
    public bool ShowStatusNote
    {
        get => _showStatusNote;
        set => SetValue(ref _showStatusNote, value);
    }

    public void ToggleStatusNote() => ShowStatusNote = !ShowStatusNote;

    public IEnumerable<TicketAttachment> Attachments => _ticket.Attachments;
    public async Task AttachmentsChanged(IEnumerable<Attachment> attachments)
    {
        await SaveTicketAsync(_ticket with
        {
            Attachments = [
                .. attachments.Select(x => new TicketAttachment
                {
                    Id = x.Id,
                    Name = x.Name,
                    Comment = x.Comment,
                    Data = x.Data,
                })
            ]
        });
    }

    private bool _isOpenAttachmentsDialog = false;
    public bool IsOpenAttachmentsDialog
    {
        get => _isOpenAttachmentsDialog;
        set => SetValue(ref _isOpenAttachmentsDialog, value);
    }

    public void OpenAttachmentsDialog() => IsOpenAttachmentsDialog = true;

    public Ticket? Ref
    {
        get => _ticket.Ref is not null ? new Ticket { Id = _ticket.Ref.Id, Title = _ticket.Ref.Title } : null;
        set => _ = SaveTicketAsync(_ticket with { Ref = value is not null ? new RefTicket { Id = value.Id, Title = value.Title } : null });
    }

    public TimeSpan TimeSpent => TimeSpan.FromTicks(_worklogs.Sum(x => x.TimeSpent.Ticks));

    private IEnumerable<Worklog> _worklogs = [];
    public IEnumerable<Worklog> Worklogs
    {
        get => _worklogs.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id);
        set => SetValue(ref _worklogs, value);
    }

    private bool _isOpenWorklogsDialog = false;
    public bool IsOpenWorklogsDialog
    {
        get => _isOpenWorklogsDialog;
        set => SetValue(ref _isOpenWorklogsDialog, value);
    }

    public void OpenWorklogsDialog() => IsOpenWorklogsDialog = true;

    public async Task LoadAsync(int ticketId)
    {
        IsLoading = true;

        try
        {
            await Task.WhenAll([
                LoadTicketAsync(ticketId),
                LoadWorklogsAsync(ticketId)
            ]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadTicketAsync(int id)
    {
        _ticket = await _dataService.GetTicketAsync(id);
    }

    private async Task LoadWorklogsAsync(int ticketId)
    {
        Worklogs = [.. (await _dataService.GetWorklogsAsync(0, 0, $"TicketId == {ticketId}")).Items];
    }

    private async Task SaveTicketAsync(Ticket ticket)
    {
        _ticket = ticket;

        await _dataService.SaveTicketAsync(ticket);
    }

    public async Task<bool> DeleteTicketAsync()
    {
        try
        {
            await _dataService.DeleteTicketAsync(_ticket.Id);
        }
        catch
        {
            _popupService.Error("Fehler beim Löschen!");
            return false;
        }

        _popupService.Info("Ticket gelöscht!");
        _navigationService.NavigateToPage("/ticket-list");

        return true;
    }

    public async Task<bool> SaveWorklogAsync(Worklog? worklog = null)
    {
        worklog ??= new Worklog
        {
            Date = DateOnly.FromDateTime(DateTimeOffset.Now.Date),
            TicketId = _ticket.Id,
            TicketTitle = _ticket.Title,
            TimeSpent = TimeSpan.Zero,
        };

        Worklog savedWorklog;

        try
        {
            savedWorklog = await _dataService.SaveWorklogAsync(worklog);
        }
        catch
        {
            _popupService.Error("Fehler beim Speichern vom Arbeitsaufwand!");
            return false;
        }

        Worklogs = [.. _worklogs.Select(x => x.Id == savedWorklog.Id ? savedWorklog : x)];

        return true;
    }

    public async Task<bool> DeleteWorklogAsync(Worklog worklog)
    {
        Worklogs = [.. _worklogs.Where(x => x.Id != worklog.Id)];

        try
        {
            await _dataService.DeleteWorklogAsync(worklog.Id);
        }
        catch
        {
            _popupService.Error("Fehler beim Löschen vom Arbeitsaufwand!");
            return false;
        }

        return true;
    }
}
