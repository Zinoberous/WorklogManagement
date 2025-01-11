using System.Collections.Concurrent;
using System.ComponentModel;
using WorklogManagement.UI.Common;

namespace WorklogManagement.UI.Services;

public interface IGlobalDataStateService : INotifyPropertyChanged
{
    bool IsLoading { get; }

    IEnumerable<string> Errors { get; }

    void StartOperation();

    void EndOperation();

    void SetError(string message);

    void ResetErrors();
}

public class GlobalDataStateService : Observable, IGlobalDataStateService
{
    private int _operationCounter = 0;
    public bool IsLoading => _operationCounter > 0;

    private readonly ConcurrentBag<string> _errors = [];
    public IEnumerable<string> Errors => _errors;

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

    public void SetError(string message) => _errors.Add(message);

    public void ResetErrors() => _errors.Clear();
}
