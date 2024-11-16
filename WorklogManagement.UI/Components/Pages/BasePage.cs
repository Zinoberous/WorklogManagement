using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using WorklogManagement.UI.ViewModels;

namespace WorklogManagement.UI.Components.Pages;

public class BasePage<TViewModel> : ComponentBase, IDisposable
    where TViewModel : BaseViewModel
{
    [Inject]
    protected TViewModel ViewModel { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;

        await base.OnInitializedAsync();
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    #region dispose

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;

            _disposed = true;
        }
    }

    #endregion
}
