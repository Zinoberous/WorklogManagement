using WorklogManagement.Data.Context;

namespace WorklogManagement.UI.ViewModels;

public class ChangelogViewModel(WorklogManagementContext context) : BaseViewModel
{
    private readonly WorklogManagementContext _context = context;
}
