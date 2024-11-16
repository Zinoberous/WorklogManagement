using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Data.Models;

namespace WorklogManagement.UI.ViewModels;

public class TrackingViewModel(WorklogManagementContext context) : BaseViewModel
{
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

    private readonly WorklogManagementContext _context = context;

    public async Task LoadWorklogAsync()
    {
        Worklog = await _context.Worklogs.FirstAsync();

        IsLoading = false;
    }
}
