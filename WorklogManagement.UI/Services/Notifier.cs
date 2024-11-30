using Microsoft.JSInterop;
using Radzen;

namespace WorklogManagement.UI.Services;

public interface INotifier
{
    Task NotifyErrorAsync(string message, Exception ex);
}

public class Notifier(NotificationService notificationService, IJSRuntime jsRuntime) : INotifier
{
    private readonly NotificationService _notificationService = notificationService;
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public async Task NotifyErrorAsync(string message, Exception ex)
    {
        _notificationService.Notify(NotificationSeverity.Error, message);

        await _jsRuntime.InvokeVoidAsync("console.error", ex.ToString());
    }
}
