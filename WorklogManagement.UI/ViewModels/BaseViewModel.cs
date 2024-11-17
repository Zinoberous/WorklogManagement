using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Web;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.ViewModels;

public class BaseViewModel : BaseNotifier, IDisposable
{
    public ObservableProperty<bool> IsLoading { get; } = new(true);
    public ObservableProperty<Exception?> LoadError { get; } = new();

    private readonly NavigationManager _navigationManager;

    private readonly ICollection<IDisposable> _subscriptions = [];

    public BaseViewModel(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;

        SubscribeToObservableProperties();
    }

    protected async Task TryLoadAsync(Func<Task> loadAsync) => await TryLoadAsync(IsLoading, LoadError, loadAsync);
    protected async Task TryLoadAsync(ObservableProperty<bool> isLoading, ObservableProperty<Exception?> loadError, Func<Task> loadAsync)
    {
        isLoading.Value = true;

        try
        {
            await loadAsync();
        }
        catch (Exception ex)
        {
            loadError.Value = ex;
        }
        finally
        {
            isLoading.Value = false;
        }
    }

    protected void UpdateQuery(string key, string? value)
    {
        Uri uri = new(_navigationManager.Uri);

        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        if (string.IsNullOrWhiteSpace(value))
        {
            queryParams.Remove(key);
        }
        else
        {
            queryParams[key] = value;
        }

        var newUri = $"{uri.GetLeftPart(UriPartial.Path)}?{queryParams}";

        _navigationManager.NavigateTo(newUri, forceLoad: false);
    }

    private void SubscribeToObservableProperties()
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ObservableProperty<>))
            .ToArray();

        foreach (var property in properties)
        {
            var observableProperty = property.GetValue(this);

            if (observableProperty is not null)
            {
                var subscribeMethod = property.PropertyType.GetMethod(nameof(IObservable<object>.Subscribe), BindingFlags.Instance | BindingFlags.NonPublic);

                if (subscribeMethod is not null)
                {
                    var subscription = (IDisposable?)subscribeMethod.Invoke(observableProperty, [(Action<string?>)OnPropertyChanged]);

                    if (subscription is not null)
                    {
                        _subscriptions.Add(subscription);
                    }
                }
            }
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
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();

            _disposed = true;
        }
    }

    #endregion
}
