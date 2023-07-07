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

    public virtual DbSet<BusTrip> BusTrips { get; set; }

    public virtual DbSet<BusTripStatus> BusTripStatuses { get; set; }

    public virtual DbSet<Coordination> Coordinations { get; set; }

    public virtual DbSet<CoordinationStatus> CoordinationStatuses { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    public virtual DbSet<RouteStation> RouteStations { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=FBus_Main:ConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC076E663318");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Code, "UQ__Account__A25C5AA7F37515B0").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Account__A9D10534F45461E6").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__Bus__3214EC07C82099B6");

            entity.ToTable("Bus");

            entity.HasIndex(e => e.LicensePlate, "UQ__Bus__026BC15C6F0F8A83").IsUnique();

            entity.HasIndex(e => e.Code, "UQ__Bus__A25C5AA74B08D7FB").IsUnique();

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
                .HasConstraintName("FK__Bus__CreatedById__4316F928");
        });

        modelBuilder.Entity<BusTrip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BusTrip__3214EC075F6F78CC");

            entity.ToTable("BusTrip");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndingDate).HasColumnType("datetime");
            entity.Property(e => e.StartingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Coordination).WithMany(p => p.BusTrips)
                .HasForeignKey(d => d.CoordinationId)
                .HasConstraintName("FK__BusTrip__Coordin__5FB337D6");
        });

        modelBuilder.Entity<BusTripStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BusTripS__3214EC0743CCA9D0");

            entity.ToTable("BusTripStatus");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(100);

            entity.HasOne(d => d.BusTrip).WithMany(p => p.BusTripStatuses)
                .HasForeignKey(d => d.BusTripId)
                .HasConstraintName("FK__BusTripSt__BusTr__6477ECF3");

            entity.HasOne(d => d.Station).WithMany(p => p.BusTripStatuses)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK__BusTripSt__Stati__656C112C");
        });

        modelBuilder.Entity<Coordination>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Coordina__3214EC07A94A3820");

            entity.ToTable("Coordination");

            entity.Property(e => e.DateLine).HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasOne(d => d.Bus).WithMany(p => p.CoordinationBus)
                .HasForeignKey(d => d.BusId)
                .HasConstraintName("FK__Coordinat__BusId__571DF1D5");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Coordinations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Coordinat__Creat__5535A963");

            entity.HasOne(d => d.Driver).WithMany(p => p.CoordinationDrivers)
                .HasForeignKey(d => d.DriverId)
                .HasConstraintName("FK__Coordinat__Drive__5629CD9C");

            entity.HasOne(d => d.Route).WithMany(p => p.CoordinationRoutes)
                .HasForeignKey(d => d.RouteId)
                .HasConstraintName("FK__Coordinat__Route__5812160E");
        });

        modelBuilder.Entity<CoordinationStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Coordina__3214EC07F16E2313");

            entity.ToTable("CoordinationStatus");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(100);

            entity.HasOne(d => d.Coordination).WithMany(p => p.CoordinationStatuses)
                .HasForeignKey(d => d.CoordinationId)
                .HasConstraintName("FK__Coordinat__Coord__5BE2A6F2");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.CoordinationStatuses)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Coordinat__Creat__5AEE82B9");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Driver__3214EC07AED4DC24");

            entity.ToTable("Driver");

            entity.HasIndex(e => e.PersonalEmail, "UQ__Driver__7B5B59A546643726").IsUnique();

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
                .HasConstraintName("FK__Driver__AccountI__3C69FB99");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.DriverCreatedBies)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Driver__CreatedB__3D5E1FD2");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Route__3214EC07C0287170");

            entity.ToTable("Route");

            entity.Property(e => e.Beginning).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Destination).HasMaxLength(100);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Routes)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Route__CreatedBy__46E78A0C");
        });

        modelBuilder.Entity<RouteStation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RouteSta__3214EC07E97BE945");

            entity.ToTable("RouteStation");

            entity.Property(e => e.StationOrder).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.Route).WithMany(p => p.RouteStations)
                .HasForeignKey(d => d.RouteId)
                .HasConstraintName("FK__RouteStat__Route__5070F446");

            entity.HasOne(d => d.Station).WithMany(p => p.RouteStations)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK__RouteStat__Stati__5165187F");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Station__3214EC072FF45454");

            entity.ToTable("Station");

            entity.HasIndex(e => e.Code, "UQ__Station__A25C5AA74650DC17").IsUnique();

            entity.Property(e => e.AddressNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(50);
            entity.Property(e => e.Image)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Street).HasMaxLength(50);
            entity.Property(e => e.Ward).HasMaxLength(50);

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.Stations)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Station__Created__4CA06362");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
