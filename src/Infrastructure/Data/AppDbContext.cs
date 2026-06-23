using DT_ASPNET.Domain.Notification;
using DT_ASPNET.Domain.Property;
using DT_ASPNET.Domain.Reservation;
using DT_ASPNET.Domain.User;
using DT_ASPNET.Domain.Wishlist;
using Microsoft.EntityFrameworkCore;

namespace DT_ASPNET.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}