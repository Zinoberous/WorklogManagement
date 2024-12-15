using Microsoft.JSInterop;
using Radzen;
using System.Collections.Concurrent;

namespace WorklogManagement.UI.Services;

public interface IToastService
{
    Task MarkRenderingCompleteAsync();

    void Info(string message);
    void Success(string message);
    void Warning(string message);
    Task Error(string message, Exception ex);
}

public class ToastService(NotificationService notificationService, IJSRuntime jsRuntime) : IToastService
{
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
