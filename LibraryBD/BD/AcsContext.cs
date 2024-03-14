using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryBD.BD;

public partial class AcsContext : DbContext
{
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

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Offender> Offenders { get; set; }

    public virtual DbSet<Personal> Personals { get; set; }

    public virtual DbSet<SubscriberTelegramBot> SubscriberTelegramBots { get; set; }

    public virtual DbSet<VideoStream> VideoStreams { get; set; }

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
            entity.Property(e => e.DirName).HasMaxLength(50);
            entity.Property(e => e.Fio)
                .HasMaxLength(100)
                .HasColumnName("FIO");
            entity.Property(e => e.Hex).HasMaxLength(50);
            entity.Property(e => e.PassOrDeny).HasMaxLength(50);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Time).HasMaxLength(50);
            entity.Property(e => e.TimeConverted).HasColumnType("datetime");
            entity.Property(e => e.W26).HasMaxLength(50);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");

            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasColumnType("text");

            entity.HasOne(d => d.Admin).WithMany(p => p.Logs)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_Log_Admin");

            entity.HasOne(d => d.Personal).WithMany(p => p.Logs)
                .HasForeignKey(d => d.PersonalId)
                .HasConstraintName("FK_Log_Personal");
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

            entity.Property(e => e.Dec).HasMaxLength(50);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Fio)
                .HasMaxLength(100)
                .HasColumnName("FIO");
            entity.Property(e => e.Hex).HasMaxLength(50);
            entity.Property(e => e.Image).HasColumnType("image");
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.W26)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SubscriberTelegramBot>(entity =>
        {
            entity.HasKey(e => e.ChatId);

            entity.ToTable("SubscriberTelegramBOT");

            entity.Property(e => e.ChatId).HasMaxLength(15);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.SubscribeOrNot).HasDefaultValueSql("((0))");
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<VideoStream>(entity =>
        {
            entity.ToTable("VideoStream");

            entity.Property(e => e.Link).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
