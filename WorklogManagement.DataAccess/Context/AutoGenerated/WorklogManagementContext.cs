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

        public virtual DbSet<Day> Days { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<Ticket> Tickets { get; set; } = null!;
        public virtual DbSet<TicketAttachment> TicketAttachments { get; set; } = null!;
        public virtual DbSet<TicketComment> TicketComments { get; set; } = null!;
        public virtual DbSet<TicketCommentAttachment> TicketCommentAttachments { get; set; } = null!;
        public virtual DbSet<Workload> Workloads { get; set; } = null!;
        public virtual DbSet<Worklog> Worklogs { get; set; } = null!;
        public virtual DbSet<WorklogAttachment> WorklogAttachments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Day>(entity =>
            {
                entity.HasOne(d => d.Workload)
                    .WithMany(p => p.Days)
                    .HasForeignKey(d => d.WorkloadId)
                    .HasConstraintName("FK_Day_WorkloadId_Workload_Id");
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

            modelBuilder.Entity<TicketAttachment>(entity =>
            {
                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketAttachments)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketAttachment_TicketId_Ticket_Id");
            });

            modelBuilder.Entity<TicketComment>(entity =>
            {
                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketComments)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketComment_TicketId_Ticket_Id");
            });

            modelBuilder.Entity<TicketCommentAttachment>(entity =>
            {
                entity.HasOne(d => d.TicketComment)
                    .WithMany(p => p.TicketCommentAttachments)
                    .HasForeignKey(d => d.TicketCommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketCommentAttachment_TicketCommentId_TicketComment_Id");
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

            modelBuilder.Entity<WorklogAttachment>(entity =>
            {
                entity.HasOne(d => d.Worklog)
                    .WithMany(p => p.WorklogAttachments)
                    .HasForeignKey(d => d.WorklogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorklogAttachment_WorklogId_Worklog_Id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
