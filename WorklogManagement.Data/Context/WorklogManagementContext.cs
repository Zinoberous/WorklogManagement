using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Models;

namespace WorklogManagement.Data.Context;

public partial class WorklogManagementContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Absence>(entity =>
        {
            entity.Ignore(x => x.DurationSpan);
        });

        modelBuilder.Entity<Worklog>(entity =>
        {
            entity.Ignore(x => x.TimeSpentSpan);
        });

        modelBuilder.Entity<WorkTime>(entity =>
        {
            entity.Ignore(x => x.ExpectedSpan);
            entity.Ignore(x => x.ActualSpan);
        });
    }
}
