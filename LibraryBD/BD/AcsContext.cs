using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryBD.BD;

public partial class AcsContext : DbContext
{
    private static AcsContext instance;

    public AcsContext()
    {
    }

    public AcsContext(DbContextOptions<AcsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Cycle> Cycles { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Offender> Offenders { get; set; }

    public virtual DbSet<Personal> Personals { get; set; }

    public virtual DbSet<Point> Points { get; set; }

    public virtual DbSet<SubscriberTelegramBot> SubscriberTelegramBots { get; set; }

    public virtual DbSet<Translate> Translates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=10.10.1.102;database=ACS;Trusted_Connection=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admin");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Patronymic).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        modelBuilder.Entity<Cycle>(entity =>
        {
            entity.ToTable("Cycle");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.TimeP1).HasColumnType("datetime");
            entity.Property(e => e.TimeP2).HasColumnType("datetime");
            entity.Property(e => e.W26).HasMaxLength(50);

            entity.HasOne(d => d.Event).WithMany(p => p.Cycles)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cycle_Event");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event");

            entity.Property(e => e.Dec).HasMaxLength(50);
            entity.Property(e => e.DirName).HasMaxLength(7);
            entity.Property(e => e.Hex).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PassDenyId)
                .HasMaxLength(7)
                .HasColumnName("PassDenyID");
            entity.Property(e => e.PointId).HasColumnName("PointID");
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.Time).HasMaxLength(50);
            entity.Property(e => e.TimeConverted).HasColumnType("datetime");
            entity.Property(e => e.W26).HasMaxLength(50);

            entity.HasOne(d => d.DirNameNavigation).WithMany(p => p.EventDirNameNavigations)
                .HasForeignKey(d => d.DirName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Translate1");

            entity.HasOne(d => d.PassDeny).WithMany(p => p.EventPassDenies)
                .HasForeignKey(d => d.PassDenyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Translate");

            entity.HasOne(d => d.Point).WithMany(p => p.Events)
                .HasForeignKey(d => d.PointId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Point");
        });

        modelBuilder.Entity<Offender>(entity =>
        {
            entity.ToTable("Offender");

            entity.Property(e => e.Dec)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Hex)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.SendOrNot).HasDefaultValueSql("((0))");
            entity.Property(e => e.Time).HasColumnType("datetime");
            entity.Property(e => e.W26)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<Personal>(entity =>
        {
            entity.ToTable("Personal");

            entity.Property(e => e.Fio)
                .HasMaxLength(100)
                .HasColumnName("FIO");
        });

        modelBuilder.Entity<Point>(entity =>
        {
            entity.ToTable("Point");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<SubscriberTelegramBot>(entity =>
        {
            entity.HasKey(e => e.ChatId);

            entity.ToTable("SubscriberTelegramBOT");

            entity.Property(e => e.ChatId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.SubscribeOrNot).HasDefaultValueSql("((0))");
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Translate>(entity =>
        {
            entity.ToTable("Translate");

            entity.Property(e => e.Id).HasMaxLength(7);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    public static AcsContext GetInstance()
    {
        if (instance == null)
        {
            instance = new AcsContext();
        }
        return instance;
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
