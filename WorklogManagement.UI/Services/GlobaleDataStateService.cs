using System.ComponentModel;
using WorklogManagement.UI.Common;

namespace WorklogManagement.UI.Services;

public interface IGlobalDataStateService : INotifyPropertyChanged
{
    bool IsLoading { get; }

    bool HasError { get; }

    void StartOperation();

    void EndOperation();

    void SetError();

    void ResetError();
}

public class GlobaleDataStateService : Observable, IGlobalDataStateService
{
    private int _operationCounter = 0;
    private bool _hasError = false;

    private readonly object _lock = new();

    public bool IsLoading => _operationCounter > 0;

    public bool HasError
    {
        get
        {
            lock (_lock)
            {
                return _hasError;
            }
        }
        private set
        {
            lock (_lock)
            {
                SetValue(ref _hasError, value);
            }
        }
    }

    public void StartOperation()
    {
        Interlocked.Increment(ref _operationCounter);
        OnPropertyChanged(nameof(IsLoading));
    }

    public void EndOperation()
    {
        if (Interlocked.Decrement(ref _operationCounter) < 0)
        {
            Interlocked.Exchange(ref _operationCounter, 0);
        }

        OnPropertyChanged(nameof(IsLoading));
    }

    public void SetError() => HasError = true;

    public void ResetError() => HasError = false;
}
