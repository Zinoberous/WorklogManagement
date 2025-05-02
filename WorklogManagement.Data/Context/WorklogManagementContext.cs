using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Models;

namespace WorklogManagement.Data.Context;

public partial class WorklogManagementContext
{
    [SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "Partial.")]
    [SuppressMessage("CodeQuality", "IDE0079:Unnötige Unterdrückung entfernen", Justification = "Fehlerhaft erkannt.")]
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
