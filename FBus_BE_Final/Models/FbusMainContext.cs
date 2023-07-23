using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FBus_BE.Models;

public partial class FbusMainContext : DbContext
{
    public FbusMainContext()
    {
    }

    public FbusMainContext(DbContextOptions<FbusMainContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Bus> Buses { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    public virtual DbSet<RouteStation> RouteStations { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<TripStatus> TripStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=FBus_Main:ConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC07CB5A6945");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Code, "UQ__Account__A25C5AA77EA5576B").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Account__A9D105345741DCC1").IsUnique();

            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Bus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bus__3214EC07C221AC3D");

            entity.ToTable("Bus");

            entity.HasIndex(e => e.LicensePlate, "UQ__Bus__026BC15C06613F3A").IsUnique();

            entity.HasIndex(e => e.Code, "UQ__Bus__A25C5AA7739E0BC4").IsUnique();

            entity.Property(e => e.Brand).HasMaxLength(20);
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateOfRegistration).HasColumnType("date");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(9)
                .IsUnicode(false);
            entity.Property(e => e.Model).HasMaxLength(30);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Buses)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Bus__CreatedById__412EB0B6");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Driver__3214EC074C890AA6");

            entity.ToTable("Driver");

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Avatar)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.IdCardNumber)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.PersonalEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(13)
                .IsUnicode(false);

            entity.HasOne(d => d.Account).WithMany(p => p.DriverAccounts)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Driver__AccountI__3A81B327");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.DriverCreatedBies)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Driver__CreatedB__3B75D760");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Route__3214EC07C8C4253A");

            entity.ToTable("Route");

            entity.Property(e => e.Beginning).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Destination).HasMaxLength(100);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Routes)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Route__CreatedBy__44FF419A");
        });

        modelBuilder.Entity<RouteStation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RouteSta__3214EC0718CD1219");

            entity.ToTable("RouteStation");

            entity.Property(e => e.StationOrder).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.Route).WithMany(p => p.RouteStations)
                .HasForeignKey(d => d.RouteId)
                .HasConstraintName("FK__RouteStat__Route__4E88ABD4");

            entity.HasOne(d => d.Station).WithMany(p => p.RouteStations)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK__RouteStat__Stati__4F7CD00D");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Station__3214EC07891D93E8");

            entity.ToTable("Station");

            entity.HasIndex(e => e.Code, "UQ__Station__A25C5AA74955F886").IsUnique();

            entity.Property(e => e.AddressNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Image)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Street).HasMaxLength(100);
            entity.Property(e => e.Ward).HasMaxLength(100);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Stations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Station__Created__4AB81AF0");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Trip__3214EC072DE1103C");

            entity.ToTable("Trip");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateLine).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasOne(d => d.Bus).WithMany(p => p.Trips)
                .HasForeignKey(d => d.BusId)
                .HasConstraintName("FK__Trip__BusId__5535A963");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Trips)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Trip__CreatedByI__534D60F1");

            entity.HasOne(d => d.Driver).WithMany(p => p.Trips)
                .HasForeignKey(d => d.DriverId)
                .HasConstraintName("FK__Trip__DriverId__5441852A");

            entity.HasOne(d => d.Route).WithMany(p => p.Trips)
                .HasForeignKey(d => d.RouteId)
                .HasConstraintName("FK__Trip__RouteId__5629CD9C");
        });

        modelBuilder.Entity<TripStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripStat__3214EC07A1893980");

            entity.ToTable("TripStatus");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.TripStatuses)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__TripStatu__Creat__59FA5E80");

            entity.HasOne(d => d.Station).WithMany(p => p.TripStatuses)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK__TripStatu__Stati__5BE2A6F2");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripStatuses)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK__TripStatu__TripI__5AEE82B9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
