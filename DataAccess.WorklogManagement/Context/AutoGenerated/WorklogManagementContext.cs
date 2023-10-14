using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DataAccess.WorklogManagement.Models;

namespace DataAccess.WorklogManagement.Context
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

        public virtual DbSet<Attachment> Attachments { get; set; } = null!;
        public virtual DbSet<Day> Days { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<StatusHistory> StatusHistories { get; set; } = null!;
        public virtual DbSet<Ticket> Tickets { get; set; } = null!;
        public virtual DbSet<Workload> Workloads { get; set; } = null!;
        public virtual DbSet<Worklog> Worklogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.HasOne(d => d.Worklog)
                    .WithMany(p => p.Attachments)
                    .HasForeignKey(d => d.WorklogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Attachment_WorklogId_Worklog_Id");
            });

            modelBuilder.Entity<Day>(entity =>
            {
                entity.HasOne(d => d.Workload)
                    .WithMany(p => p.Days)
                    .HasForeignKey(d => d.WorkloadId)
                    .HasConstraintName("FK_Day_WorkloadId_Workload_Id");
            });

            modelBuilder.Entity<StatusHistory>(entity =>
            {
                entity.HasOne(d => d.Status)
                    .WithMany(p => p.StatusHistories)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StatusHistory_StatusId_Status_Id");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.StatusHistories)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StatusHistory_TicketId_Ticket_Id");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasOne(d => d.Ref)
                    .WithMany(p => p.InverseRef)
                    .HasForeignKey(d => d.RefId)
                    .HasConstraintName("FK_Ticket_RefId_Ticket_Id");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_StatusId_Status_Id");
            });

            modelBuilder.Entity<Worklog>(entity =>
            {
                entity.HasOne(d => d.Day)
                    .WithMany(p => p.Worklogs)
                    .HasForeignKey(d => d.DayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Worklog_DayId_Day_Id");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.Worklogs)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Worklog_TicketId_Ticket_Id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
