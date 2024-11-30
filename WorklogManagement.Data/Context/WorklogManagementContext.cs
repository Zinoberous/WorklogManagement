using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Models;

namespace WorklogManagement.Data.Context;

public partial class WorklogManagementContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Worklog>(entity =>
        {
            entity.Ignore(x => x.TimeSpentMinutes);
        });
    }
}
