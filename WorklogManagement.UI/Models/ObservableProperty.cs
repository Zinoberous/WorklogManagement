using Reactive.Bindings;

namespace WorklogManagement.UI.Models;

public class ObservableProperty<T>(T initialValue = default!)
{
    private readonly ReactiveProperty<T> _reactiveProperty = new(initialValue);

    public T Value
    {
        get => _reactiveProperty.Value;
        set => _reactiveProperty.Value = value;
    }

    internal IDisposable Subscribe(Action<string?> onPropertyChanged)
    {
        return _reactiveProperty.Subscribe(_ => onPropertyChanged(nameof(Value)));
    }
}
