using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.ComponentModel;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Inject]
    public required IGlobalDataStateService DataStateService { get; set; }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
        DataStateService.PropertyChanged += OnPropertyChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        DataStateService.ResetError();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
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
            NavigationManager.LocationChanged -= OnLocationChanged;
            DataStateService.PropertyChanged -= OnPropertyChanged;

            _disposed = true;
        }
    }

    #endregion
}