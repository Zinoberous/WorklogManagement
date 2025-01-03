using System.Collections.Concurrent;
using System.ComponentModel;
using WorklogManagement.UI.Common;

namespace WorklogManagement.UI.Services;

public interface IGlobalDataStateService : INotifyPropertyChanged
{
    bool IsLoading { get; }

    IEnumerable<Exception> Errors { get; }

    void StartOperation();

    void EndOperation();

    void SetError(Exception ex);

    void ResetErrors();
}

public class GlobalDataStateService : Observable, IGlobalDataStateService
{
    private int _operationCounter = 0;
    public bool IsLoading => _operationCounter > 0;

    private ConcurrentBag<Exception> _errors = [];
    public IEnumerable<Exception> Errors => _errors;

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

    public void SetError(Exception ex) => _errors.Add(ex);

    public void ResetErrors() => _errors.Clear();
}
