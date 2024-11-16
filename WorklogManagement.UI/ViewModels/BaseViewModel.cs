using System.Reflection;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.ViewModels;

public class BaseViewModel : BaseNotifier, IDisposable
{
    private readonly ICollection<IDisposable> _subscriptions = [];

    public BaseViewModel()
    {
        SubscribeToObservableProperties();
    }

    private void SubscribeToObservableProperties()
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
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
