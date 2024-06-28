using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HenryMedsApp.Models;

public partial class HenryMedsContext : DbContext
{
    public HenryMedsContext()
    {
    }

    public HenryMedsContext(DbContextOptions<HenryMedsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProviderSchedule> ProviderSchedules { get; set; }

    public virtual DbSet<ClientBooking> ClientBookings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=HenryMeds;Trusted_Connection=true;Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProviderSchedule>(entity =>
        {
            entity.ToTable("ProviderSchedule");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ClientBooking>(entity =>
        {
            entity.ToTable("ClientBooking");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
