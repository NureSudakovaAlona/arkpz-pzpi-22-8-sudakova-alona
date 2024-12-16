using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FocusLearn.Models.Domain;

public partial class FocusLearnDbContext : DbContext
{
    public FocusLearnDbContext()
    {
    }

    public FocusLearnDbContext(DbContextOptions<FocusLearnDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<ConcentrationMethod> ConcentrationMethods { get; set; }

    public virtual DbSet<IoTsession> IoTsessions { get; set; }

    public virtual DbSet<LearningMaterial> LearningMaterials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserMethodStatistic> UserMethodStatistics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=F15_PLUS_2;Database=FocusLearnDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__32499E771B0A7554");

            entity.ToTable("Assignment");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Rating).HasColumnType("tinyint");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Student).WithMany(p => p.AssignmentStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Assignmen__Stude__6FE99F9F");

            entity.HasOne(d => d.Tutor).WithMany(p => p.AssignmentTutors)
                .HasForeignKey(d => d.TutorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Assignmen__Tutor__70DDC3D8");

            entity.HasOne(d => d.Task).WithMany(p => p.AssignmentLearningMaterials)
               .HasForeignKey(d => d.TaskId)
               .HasConstraintName("FK__Assignmen__TaskI__14270015");
        });

        modelBuilder.Entity<ConcentrationMethod>(entity =>
        {
            entity.HasKey(e => e.MethodId).HasName("PK__Concentr__FC68185194ECBAE0");

            entity.ToTable("ConcentrationMethod");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<IoTsession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__IoTSessi__C9F492900013076F");

            entity.ToTable("IoTSession");

            entity.Property(e => e.Duration).HasComputedColumnSql("(datediff(minute,[StartTime],[EndTime]))", false);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.SessionType).HasMaxLength(20);
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Method).WithMany(p => p.IoTsessions)
                .HasForeignKey(d => d.MethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__IoTSessio__Metho__7B5B524B");

            entity.HasOne(d => d.User).WithMany(p => p.IoTsessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__IoTSessio__UserI__7A672E12");
        });

        modelBuilder.Entity<LearningMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Learning__C50610F735EF2B4E");

            entity.ToTable("LearningMaterial");

            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Creator).WithMany(p => p.LearningMaterials)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LearningM__Creat__4AB81AF0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C1B4F9E18");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534B41D86F3").IsUnique();

            entity.Property(e => e.AuthProvider).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Language).HasMaxLength(20);
            entity.Property(e => e.ProfileStatus).HasMaxLength(20);
            entity.Property(e => e.ProviderId).HasMaxLength(100);
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserMethodStatistic>(entity =>
        {
            entity.HasKey(e => e.StatisticId).HasName("PK__UserMeth__367DEB177AC97C1A");

            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PeriodStartDate).HasColumnType("date");
            entity.Property(e => e.PeriodType).HasMaxLength(10);

            entity.HasOne(d => d.Method).WithMany(p => p.UserMethodStatistics)
                .HasForeignKey(d => d.MethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserMetho__Metho__02FC7413");

            entity.HasOne(d => d.User).WithMany(p => p.UserMethodStatistics)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserMetho__UserI__02084FDA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
