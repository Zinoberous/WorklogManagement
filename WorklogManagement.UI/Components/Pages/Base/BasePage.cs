using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace WorklogManagement.UI.Components.Pages.Base;

public class BasePage<TViewModel> : ComponentBase, IDisposable
    where TViewModel : BaseViewModel
{
    [Inject]
    protected TViewModel ViewModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        RegisterPropertyChanged();
    }

    private void RegisterPropertyChanged()
    {
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
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
