using Microsoft.JSInterop;
using Radzen;
using System.Collections.Concurrent;

namespace WorklogManagement.UI.Services;

public interface IPopupService
{
    Task MarkRenderingCompleteAsync();

    Task Alert(string title, string message);
    Task<bool> Confim(string title, string message);

    void Info(string message);
    void Success(string message);
    void Warning(string message);
    Task Error(string message, Exception ex);
}

public class PopupService(DialogService dialogService, NotificationService notificationService, IJSRuntime jsRuntime) : IPopupService
{
    private readonly DialogService _dialogService = dialogService;
    private readonly NotificationService _notificationService = notificationService;
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    private static readonly ConcurrentBag<Exception> _pendingErrors = [];

    private bool _isFirstRenderingComplete;

    public async Task MarkRenderingCompleteAsync()
    {
        _isFirstRenderingComplete = true;

        foreach (var ex in _pendingErrors)
        {
            await NotifyErrorToConsoleAsync(ex);
        }
    }

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

    public async Task Error(string message, Exception ex)
    {
        _notificationService.Notify(NotificationSeverity.Error, message);

        if (_isFirstRenderingComplete)
        {
            await NotifyErrorToConsoleAsync(ex);
        }
        else
        {
            _pendingErrors.Add(ex);
        }
    }

    private async Task NotifyErrorToConsoleAsync(Exception ex)
    {
        await _jsRuntime.InvokeVoidAsync("console.error", ex.ToString());
    }
}
