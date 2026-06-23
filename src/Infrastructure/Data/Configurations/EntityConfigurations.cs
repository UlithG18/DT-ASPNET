using DT_ASPNET.Domain.Notification;
using DT_ASPNET.Domain.Property;
using DT_ASPNET.Domain.Reservation;
using DT_ASPNET.Domain.User;
using DT_ASPNET.Domain.Wishlist;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT_ASPNET.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.KycStatus).HasConversion<string>();
        builder.Property(u => u.KycDocumentNumber).HasMaxLength(50);
        builder.Property(u => u.KycRejectionReason).HasMaxLength(500);
        builder.Property(u => u.RefreshToken).HasMaxLength(512);
    }
}

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("properties");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.City).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
        builder.Property(p => p.PricePerNight).HasPrecision(12, 2);

        // Fotos como array de texto en Postgres — sin tabla extra
        builder.Property(p => p.PhotoUrls)
            .HasColumnType("text[]");

        builder.HasIndex(p => p.City);
    }
}

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.PricePerNight).HasPrecision(12, 2);
        builder.Property(r => r.TotalAmount).HasPrecision(12, 2);
        builder.Property(r => r.Status).HasConversion<string>();
        builder.Property(r => r.CancellationReason).HasMaxLength(500);

        // Índice para consultas de disponibilidad
        builder.HasIndex(r => new { r.PropertyId, r.CheckIn, r.CheckOut });
    }
}

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("wishlists");
        builder.HasKey(w => w.Id);

        // Un usuario no puede guardar el mismo inmueble dos veces
        builder.HasIndex(w => new { w.UserId, w.PropertyId }).IsUnique();
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Body).IsRequired();
        builder.Property(n => n.Type).HasConversion<string>();

        builder.HasIndex(n => new { n.UserId, n.IsRead });
    }
}