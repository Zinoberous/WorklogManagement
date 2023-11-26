using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.DataAccess.Models;

namespace WorklogManagement.DataAccess.Context;

public partial class WorklogManagementContext : DbContext
{
    public WorklogManagementContext(DbContextOptions<WorklogManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CalendarEntry> CalendarEntries { get; set; }

    public virtual DbSet<CalendarEntryType> CalendarEntryTypes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }

    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }

    public virtual DbSet<TicketStatusLog> TicketStatusLogs { get; set; }

    public virtual DbSet<Worklog> Worklogs { get; set; }

    public virtual DbSet<WorklogAttachment> WorklogAttachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalendarEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CalendarEntry_Id");

            entity.HasOne(d => d.CalendarEntryType).WithMany(p => p.CalendarEntries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalendarEntry_Date_CalendarEntryType_Id");
        });

        modelBuilder.Entity<CalendarEntryType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CalendarEntryType_Id");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Ticket_Id");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Ref).WithMany(p => p.InverseRef).HasConstraintName("FK_Ticket_RefId_Ticket_Id");

            entity.HasOne(d => d.TicketStatus).WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_TicketStatusId_TicketStatus_Id");
        });

        modelBuilder.Entity<TicketAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TicketAttachment_Id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketAttachments).HasConstraintName("FK_TicketAttachment_TicketId_Ticket_Id");
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TicketStatus_Id");
        });

        modelBuilder.Entity<TicketStatusLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TicketStatusLog_Id");

            entity.Property(e => e.StartedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketStatusLogs).HasConstraintName("FK_TicketStatusLog_TicketId_Ticket_Id");

            entity.HasOne(d => d.TicketStatus).WithMany(p => p.TicketStatusLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketStatusLog_TicketStatusId_TicketStatus_Id");
        });

        modelBuilder.Entity<Worklog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Worklog_Id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Worklogs).HasConstraintName("FK_Worklog_TicketId_Ticket_Id");
        });

        modelBuilder.Entity<WorklogAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WorklogAttachment_Id");

            entity.HasOne(d => d.Worklog).WithMany(p => p.WorklogAttachments).HasConstraintName("FK_WorklogAttachment_WorklogId_Worklog_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
