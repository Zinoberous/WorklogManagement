using WorklogManagement.Data.Models;

namespace WorklogManagement.Data;

public interface IWorklogManagementService
{
    Task<Worklog> GetFirstWorklogAsync();
}
