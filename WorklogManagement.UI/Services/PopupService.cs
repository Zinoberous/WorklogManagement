using Radzen;

namespace WorklogManagement.UI.Services;

public interface IPopupService
{
    Task Alert(string title, string message);
    Task<bool> Confim(string title, string message);

    void Info(string message);
    void Success(string message);
    void Warning(string message);
    void Error(string message);
}

public class PopupService(DialogService dialogService, NotificationService notificationService) : IPopupService
{
    private readonly DialogService _dialogService = dialogService;
    private readonly NotificationService _notificationService = notificationService;

    public async Task Alert(string title, string message)
    {
        await _dialogService.Alert(message, title, new() { OkButtonText = "Ja" });
    }

    public async Task<bool> Confim(string title, string message)
    {
        return await _dialogService.Confirm(message, title, new() { OkButtonText = "Ja", CancelButtonText = "Nein" }) ?? false;
    }

    public void Info(string message)
    {
        _notificationService.Notify(NotificationSeverity.Info, message);
    }

    public void Success(string message)
    {
        _notificationService.Notify(NotificationSeverity.Success, message);
    }

    public void Warning(string message)
    {
        _notificationService.Notify(NotificationSeverity.Warning, message);
    }

    public void Error(string message)
    {
        _notificationService.Notify(NotificationSeverity.Error, message);
    }
}
