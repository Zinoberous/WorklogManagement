using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.ComponentModel;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public required IPopupService PopupService { get; set; }

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
        InvokeAsync(StateHasChanged);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await PopupService.MarkRenderingCompleteAsync();
        }
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