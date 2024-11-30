using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Models;

namespace WorklogManagement.Data.Context;

public partial class WorklogManagementContext : DbContext
{
    public WorklogManagementContext(DbContextOptions<WorklogManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Absence> Absences { get; set; }

    public virtual DbSet<AbsenceType> AbsenceTypes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }

    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }

    public virtual DbSet<TicketStatusLog> TicketStatusLogs { get; set; }

    public virtual DbSet<WorkTime> WorkTimes { get; set; }

    public virtual DbSet<WorkTimeType> WorkTimeTypes { get; set; }

    public virtual DbSet<Worklog> Worklogs { get; set; }

    public virtual DbSet<WorklogAttachment> WorklogAttachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Absence>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Absence_Id");

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.AbsenceType).WithMany(p => p.Absences)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Absence_AbsenceTypeId_AbsenceType_Id");
        });

        modelBuilder.Entity<AbsenceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AbsenceType_Id");

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Ticket_Id");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Ref).WithMany(p => p.InverseRef).HasConstraintName("FK_Ticket_RefId_Ticket_Id");

            entity.HasOne(d => d.TicketStatus).WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_TicketStatusId_TicketStatus_Id");
        });

        modelBuilder.Entity<TicketAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TicketAttachment_Id");

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketAttachments).HasConstraintName("FK_TicketAttachment_TicketId_Ticket_Id");
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TicketStatus_Id");

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<TicketStatusLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TicketStatusLog_Id");

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.StartedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketStatusLogs).HasConstraintName("FK_TicketStatusLog_TicketId_Ticket_Id");

            entity.HasOne(d => d.TicketStatus).WithMany(p => p.TicketStatusLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketStatusLog_TicketStatusId_TicketStatus_Id");
        });

        modelBuilder.Entity<WorkTime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WorkTime_Id");

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.WorkTimeType).WithMany(p => p.WorkTimes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkTime_WorkTimeTypeId_WorkTimeType_Id");
        });

        modelBuilder.Entity<WorkTimeType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WorkTimeType_Id");

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Worklog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Worklog_Id");

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Ticket).WithMany(p => p.Worklogs).HasConstraintName("FK_Worklog_TicketId_Ticket_Id");
        });

        modelBuilder.Entity<WorklogAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WorklogAttachment_Id");

            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Worklog).WithMany(p => p.WorklogAttachments).HasConstraintName("FK_WorklogAttachment_WorklogId_Worklog_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
