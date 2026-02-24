using Microsoft.EntityFrameworkCore;

namespace CarRental.Models;

public partial class CarRentalContext : DbContext
{
    public CarRentalContext()
    {
    }

    public CarRentalContext(DbContextOptions<CarRentalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agreement> Agreements { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<FuelType> FuelTypes { get; set; }

    public virtual DbSet<MaintenanceLog> MaintenanceLogs { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }


    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Agreement>(entity =>
    //    {
    //        entity.HasKey(e => e.AgreementId).HasName("PK__Agreemen__0A309D2341348C53");

    //        entity.ToTable("Agreement");

    //        entity.Property(e => e.AgreementId).HasColumnName("AgreementID");
    //        entity.Property(e => e.AgreementDate).HasColumnType("timestamp");
    //        entity.Property(e => e.AgreementPdfpath)
    //            .IsUnicode(false)
    //            .HasColumnName("AgreementPDFPath");
    //        entity.Property(e => e.BookingId).HasColumnName("BookingID");
    //        entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
    //        entity.Property(e => e.Created)
    //            .HasDefaultValueSql("CURRENT_TIMESTAMP")
    //            .HasColumnType("timestamp");
    //        entity.Property(e => e.Modified).HasColumnType("timestamp");
    //        entity.Property(e => e.TermsAccepted).HasDefaultValue(false);

    //        entity.HasOne(d => d.Booking)
    //            .WithMany(p => p.Agreements)
    //            .HasForeignKey(d => d.BookingId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Agreement__Booki__0A9D95DB");

    //        entity.HasOne(d => d.Customer)
    //            .WithMany(p => p.Agreements)
    //            .HasForeignKey(d => d.CustomerId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Agreement__CustomerID__0B91BA14");
    //    });

    //    modelBuilder.Entity<Booking>(entity =>
    //{
    //    entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951ACD3F96F744");

    //    entity.ToTable("Booking");

    //    entity.Property(e => e.BookingId).HasColumnName("BookingID");
    //    entity.Property(e => e.Created)
    //        .HasDefaultValueSql("CURRENT_TIMESTAMP")
    //        .HasColumnType("timestamp");
    //    entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
    //    entity.Property(e => e.EndDateTime).HasColumnType("timestamp");
    //    entity.Property(e => e.Modified).HasColumnType("timestamp");
    //    entity.Property(e => e.StartDateTime).HasColumnType("timestamp");
    //    entity.Property(e => e.Status)
    //        .HasMaxLength(50)
    //        .IsUnicode(false);
    //    entity.Property(e => e.VehicleId).HasColumnName("VehicleID");

    //    entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
    //        .HasForeignKey(d => d.CustomerId)
    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //        .HasConstraintName("FK__Booking__Custome__03F0984C");

    //    entity.HasOne(d => d.Vehicle).WithMany(p => p.Bookings)
    //        .HasForeignKey(d => d.VehicleId)
    //        .OnDelete(DeleteBehavior.ClientSetNull)
    //        .HasConstraintName("FK__Booking__Vehicle__05D8E0BE");
    //});

    //    modelBuilder.Entity<Customer>(entity =>
    //    {
    //        entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B85BE3B4A0");

    //        entity.ToTable("Customer");

    //        entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
    //        entity.Property(e => e.Created)
    //            .HasDefaultValueSql("CURRENT_TIMESTAMP")
    //            .HasColumnType("timestamp");
    //        entity.Property(e => e.Email)
    //            .HasMaxLength(100)
    //            .IsUnicode(false);
    //        entity.Property(e => e.FullName)
    //            .HasMaxLength(100)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Modified).HasColumnType("timestamp");
    //        entity.Property(e => e.Phone)
    //            .HasMaxLength(20)
    //            .IsUnicode(false);
    //    });

    //    modelBuilder.Entity<FuelType>(entity =>
    //    {
    //        entity.HasKey(e => e.FuelTypeId).HasName("PK__FuelType__048BEE575B9E6990");

    //        entity.ToTable("FuelType");

    //        entity.Property(e => e.FuelTypeId).HasColumnName("FuelTypeID");
    //        entity.Property(e => e.Created)
    //            .HasDefaultValueSql("CURRENT_TIMESTAMP")
    //            .HasColumnType("timestamp");
    //        entity.Property(e => e.FuelName)
    //            .HasMaxLength(50)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Modified).HasColumnType("timestamp");
    //        entity.Property(e => e.UserId).HasColumnName("UserID");

    //        entity.HasOne(d => d.User).WithMany(p => p.FuelTypes)
    //            .HasForeignKey(d => d.UserId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__FuelType__UserID__7A672E12");
    //    });

    //    modelBuilder.Entity<MaintenanceLog>(entity =>
    //    {
    //        entity.HasKey(e => e.LogId).HasName("PK__Maintena__5E5499A87A47E4D4");

    //        entity.ToTable("MaintenanceLog");

    //        entity.Property(e => e.LogId).HasColumnName("LogID");
    //        entity.Property(e => e.Cost).HasColumnType("decimal(10, 2)");
    //        entity.Property(e => e.Created)
    //            .HasDefaultValueSql("CURRENT_TIMESTAMP")
    //            .HasColumnType("timestamp");
    //        entity.Property(e => e.Description).HasColumnType("text");
    //        entity.Property(e => e.Modified).HasColumnType("timestamp");
    //        entity.Property(e => e.ServiceDate).HasColumnType("timestamp");
    //        entity.Property(e => e.UserId).HasColumnName("UserID");
    //        entity.Property(e => e.VehicleId).HasColumnName("VehicleID");

    //        entity.HasOne(d => d.User).WithMany(p => p.MaintenanceLogs)
    //            .HasForeignKey(d => d.UserId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Maintenan__UserI__151B244E");

    //        entity.HasOne(d => d.Vehicle).WithMany(p => p.MaintenanceLogs)
    //            .HasForeignKey(d => d.VehicleId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Maintenan__Vehic__14270015");
    //    });

    //    modelBuilder.Entity<Payment>(entity =>
    //    {
    //        entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A58F67A04F2");

    //        entity.ToTable("Payment");

    //        entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
    //        entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
    //        entity.Property(e => e.BookingId).HasColumnName("BookingID");
    //        entity.Property(e => e.Created)
    //            .HasDefaultValueSql("CURRENT_TIMESTAMP")
    //            .HasColumnType("timestamp");
    //        entity.Property(e => e.Modified).HasColumnType("timestamp");
    //        entity.Property(e => e.PaymentDate).HasColumnType("timestamp");
    //        entity.Property(e => e.PaymentMethod)
    //            .HasMaxLength(50)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Remarks).HasColumnType("text");
    //        entity.Property(e => e.UserId).HasColumnName("UserID");

    //        entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
    //            .HasForeignKey(d => d.BookingId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Payment__Booking__0F624AF8");

    //        entity.HasOne(d => d.User).WithMany(p => p.Payments)
    //            .HasForeignKey(d => d.UserId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Payment__UserID__10566F31");
    //    });

    //    modelBuilder.Entity<User>(entity =>
    //    {
    //        entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACBAD0D7C1");

    //        entity.ToTable("User");

    //        entity.Property(e => e.UserId).HasColumnName("UserID");
    //        entity.Property(e => e.Created)
    //            .HasDefaultValueSql("CURRENT_TIMESTAMP")
    //            .HasColumnType("timestamp");
    //        entity.Property(e => e.Email)
    //            .HasMaxLength(100)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Modified).HasColumnType("timestamp");
    //        entity.Property(e => e.Name)
    //            .HasMaxLength(100)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Password)
    //            .HasMaxLength(10)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Phone)
    //            .HasMaxLength(20)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Role)
    //            .HasMaxLength(10)
    //            .IsUnicode(false);
    //    });

    //    modelBuilder.Entity<Vehicle>(entity =>
    //    {
    //        entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B54B288194003");

    //        entity.ToTable("Vehicle");

    //        entity.Property(e => e.VehicleId).HasColumnName("VehicleID");
    //        entity.Property(e => e.Brand)
    //            .HasMaxLength(50)
    //            .IsUnicode(false);
    //        entity.Property(e => e.ConditionNote).HasColumnType("text");
    //        entity.Property(e => e.Created)
    //            .HasDefaultValueSql("CURRENT_TIMESTAMP")
    //            .HasColumnType("timestamp");
    //        entity.Property(e => e.FuelTypeId).HasColumnName("FuelTypeID");
    //        entity.Property(e => e.Model)
    //            .HasMaxLength(50)
    //            .IsUnicode(false);
    //        entity.Property(e => e.Modified).HasColumnType("timestamp");
    //        entity.Property(e => e.PlateNumber)
    //            .HasMaxLength(20)
    //            .IsUnicode(false);
    //        entity.Property(e => e.PricePerDay).HasColumnType("decimal(10, 2)");
    //        entity.Property(e => e.PricePerHour).HasColumnType("decimal(10, 2)");
    //        entity.Property(e => e.Status)
    //            .HasMaxLength(50)
    //            .IsUnicode(false);
    //        entity.Property(e => e.UserId).HasColumnName("UserID");
    //        entity.Property(e => e.VehicleTypeId).HasColumnName("VehicleTypeID");

    //        entity.HasOne(d => d.FuelType).WithMany(p => p.Vehicles)
    //            .HasForeignKey(d => d.FuelTypeId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Vehicle__FuelTyp__7E37BEF6");

    //        entity.HasOne(d => d.User).WithMany(p => p.Vehicles)
    //            .HasForeignKey(d => d.UserId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Vehicle__UserID__00200768");

    //        entity.HasOne(d => d.VehicleType).WithMany(p => p.Vehicles)
    //            .HasForeignKey(d => d.VehicleTypeId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__Vehicle__Vehicle__7F2BE32F");
    //    });

    //    modelBuilder.Entity<VehicleType>(entity =>
    //    {
    //        entity.HasKey(e => e.VehicleTypeId).HasName("PK__VehicleT__9F44962332842365");

    //        entity.ToTable("VehicleType");

    //        entity.Property(e => e.VehicleTypeId).HasColumnName("VehicleTypeID");
    //        entity.Property(e => e.Created)
    //            .HasDefaultValueSql("CURRENT_TIMESTAMP")
    //            .HasColumnType("timestamp");
    //        entity.Property(e => e.Modified).HasColumnType("timestamp");
    //        entity.Property(e => e.TypeName)
    //            .HasMaxLength(50)
    //            .IsUnicode(false);
    //        entity.Property(e => e.UserId).HasColumnName("UserID");

    //        entity.HasOne(d => d.User).WithMany(p => p.VehicleTypes)
    //            .HasForeignKey(d => d.UserId)
    //            .OnDelete(DeleteBehavior.ClientSetNull)
    //            .HasConstraintName("FK__VehicleTy__UserI__76969D2E");
    //    });

    //    OnModelCreatingPartial(modelBuilder);
    //}

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
