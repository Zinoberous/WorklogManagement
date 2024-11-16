using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Data.Models;

namespace WorklogManagement.Data;

public class WorklogManagementService(WorklogManagementContext context) : IWorklogManagementService
{
    private readonly WorklogManagementContext _context = context;

    public async Task<Worklog> GetFirstWorklogAsync()
    {
        return await _context.Worklogs.FirstAsync();
    }
}
