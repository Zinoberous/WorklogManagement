using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorklogManagement.UI.Common;

public class Observable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    protected bool SetValue<T>(ref T backingFiled, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingFiled, value))
        {
            return false;
        }

        backingFiled = value;
        OnPropertyChanged(propertyName);

        return true;
    }
}
