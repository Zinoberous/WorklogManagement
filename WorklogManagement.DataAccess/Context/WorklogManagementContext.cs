using Microsoft.EntityFrameworkCore;
using WorklogManagement.DataAccess.Models;

namespace WorklogManagement.DataAccess.Context
{
    public partial class WorklogManagementContext : DbContext
    {
        public WorklogManagementContext()
        {
        }

        public WorklogManagementContext(DbContextOptions<WorklogManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CalendarEntry> CalendarEntries { get; set; } = null!;
        public virtual DbSet<CalendarEntryType> CalendarEntryTypes { get; set; } = null!;
        public virtual DbSet<Ticket> Tickets { get; set; } = null!;
        public virtual DbSet<TicketAttachment> TicketAttachments { get; set; } = null!;
        public virtual DbSet<TicketStatus> TicketStatuses { get; set; } = null!;
        public virtual DbSet<TicketStatusLog> TicketStatusLogs { get; set; } = null!;
        public virtual DbSet<Worklog> Worklogs { get; set; } = null!;
        public virtual DbSet<WorklogAttachment> WorklogAttachments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarEntry>(entity =>
            {
                entity.HasOne(d => d.CalendarEntryType)
                    .WithMany(p => p.CalendarEntries)
                    .HasForeignKey(d => d.CalendarEntryTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("UX_Day_Date_CalendarEntryType_CalendarEntryTypeId");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Ref)
                    .WithMany(p => p.InverseRef)
                    .HasForeignKey(d => d.RefId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Ticket_RefId_Ticket_Id");

                entity.HasOne(d => d.TicketStatus)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.TicketStatusId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ticket_TicketStatusId_TicketStatus_Id");
            });

            modelBuilder.Entity<TicketAttachment>(entity =>
            {
                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketAttachments)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TicketAttachment_TicketId_Ticket_Id");
            });

            modelBuilder.Entity<TicketStatusLog>(entity =>
            {
                entity.Property(e => e.StartedAt).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketStatusLogs)
                    .HasForeignKey(d => d.TicketId)
                    .HasConstraintName("FK_TicketStatusLog_TicketId_Ticket_Id");

                entity.HasOne(d => d.TicketStatus)
                    .WithMany(p => p.TicketStatusLogs)
                    .HasForeignKey(d => d.TicketStatusId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TicketStatusLog_TicketStatusId_TicketStatus_Id");
            });

            modelBuilder.Entity<Worklog>(entity =>
            {
                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.Worklogs)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Worklog_TicketId_Ticket_Id");
            });

            modelBuilder.Entity<WorklogAttachment>(entity =>
            {
                entity.HasOne(d => d.Worklog)
                    .WithMany(p => p.WorklogAttachments)
                    .HasForeignKey(d => d.WorklogId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WorklogAttachment_WorklogId_Worklog_Id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
