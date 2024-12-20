using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Models;

namespace WorklogManagement.Data.Context;

public partial class WorklogManagementContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Absence>(entity =>
        {
            entity.Ignore(x => x.Duration);
        });

        modelBuilder.Entity<Worklog>(entity =>
        {
            entity.Ignore(x => x.TimeSpent);
        });

        modelBuilder.Entity<WorkTime>(entity =>
        {
            entity.Ignore(x => x.Expected);
            entity.Ignore(x => x.Actual);
        });
    }
}
