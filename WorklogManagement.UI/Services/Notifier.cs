using Microsoft.JSInterop;
using Radzen;
using System.Collections.Concurrent;

namespace WorklogManagement.UI.Services;

public interface INotifier
{
    Task MarkRenderingCompleteAsync();

    Task NotifyErrorAsync(string message, Exception ex);
}

public class Notifier(NotificationService notificationService, IJSRuntime jsRuntime) : INotifier
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

    public async Task NotifyErrorAsync(string message, Exception ex)
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
