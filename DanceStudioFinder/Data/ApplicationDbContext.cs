using System;
using System.Collections.Generic;
using DanceStudioFinder.Models;
using Microsoft.EntityFrameworkCore;

namespace DanceStudioFinder.Data;

public partial class ApplicationDbContext: DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AgeLimit> AgeLimits { get; set; }

    public virtual DbSet<DanceGroup> DanceGroups { get; set; }

    public virtual DbSet<DanceStudio> DanceStudios { get; set; }

    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Style> Styles { get; set; }

    public virtual DbSet<WeekDay> WeekDays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.IdAddress).HasName("address_pkey");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.IdAdmin).HasName("admin_pkey");
        });

        modelBuilder.Entity<AgeLimit>(entity =>
        {
            entity.HasKey(e => e.IdAgeLimit).HasName("age_limit_pkey");
        });

        modelBuilder.Entity<DanceGroup>(entity =>
        {
            entity.HasKey(e => e.IdGroup).HasName("dance_group_pkey");

            entity.HasOne(d => d.IdAgeLimitNavigation).WithMany(p => p.DanceGroups).HasConstraintName("dance_group_id_age_limit_fkey");

            entity.HasOne(d => d.IdStudioNavigation).WithMany(p => p.DanceGroups).HasConstraintName("dance_group_id_studio_fkey");

            entity.HasOne(d => d.IdStyleNavigation).WithMany(p => p.DanceGroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dance_group_id_style_fkey");
        });

        modelBuilder.Entity<DanceStudio>(entity =>
        {
            entity.HasKey(e => e.IdStudio).HasName("dance_studio_pkey");

            entity.HasOne(d => d.IdAddressNavigation).WithMany(p => p.DanceStudios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dance_studio_address");

            entity.HasOne(d => d.IdAdminNavigation).WithMany(p => p.DanceStudios).HasConstraintName("dance_studio_id_admin_fkey");
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.HasKey(e => e.IdPrice).HasName("prices_pkey");

            entity.HasOne(d => d.IdStudioNavigation).WithMany(p => p.Prices).HasConstraintName("prices_id_studio_fkey");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.IdSchedule).HasName("schedule_pkey");

            entity.HasOne(d => d.IdDayNavigation).WithMany(p => p.Schedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("schedule_id_day_fkey");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.Schedules).HasConstraintName("schedule_id_group_fkey");
        });

        modelBuilder.Entity<Style>(entity =>
        {
            entity.HasKey(e => e.IdStyle).HasName("style_pkey");
        });

        modelBuilder.Entity<WeekDay>(entity =>
        {
            entity.HasKey(e => e.IdDay).HasName("week_days_pkey");
        });

        //OnModelCreatingPartial(modelBuilder);
    }

    //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
