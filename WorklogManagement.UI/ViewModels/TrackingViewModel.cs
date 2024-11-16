using WorklogManagement.Data;
using WorklogManagement.Data.Models;

namespace WorklogManagement.UI.ViewModels;

public class TrackingViewModel(IWorklogManagementService service) : BaseViewModel
{
    private readonly IWorklogManagementService _service = service;

    public bool _isLoading = true;
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }

    private Worklog? _worklog;
    public Worklog? Worklog
    {
        get => _worklog;
        private set
        {
            if (_worklog != value)
            {
                _worklog = value;
                OnPropertyChanged();
            }
        }
    }

    public async Task LoadWorklogAsync()
    {
        Worklog = await _service.GetFirstWorklogAsync();

        IsLoading = false;
    }
}
