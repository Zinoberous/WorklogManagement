using Radzen;

namespace WorklogManagement.UI.Services;

public interface IPopupService
{
    Task<bool> Confim(string title, string message);
}

public class PopupService(DialogService dialogService) : IPopupService
{
    private readonly DialogService _dialogService = dialogService;

    public async Task<bool> Confim(string title, string message)
    {
        return await _dialogService.Confirm(message, title, new() { OkButtonText = "Ja", CancelButtonText = "Nein" }) ?? false;
    }
}
