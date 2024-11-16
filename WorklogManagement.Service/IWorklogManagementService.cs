using WorklogManagement.Service.Models;

namespace WorklogManagement.Service;

public interface IWorklogManagementService
{
    Task<OvertimeInfo> GetOvertimeAsync();
}
