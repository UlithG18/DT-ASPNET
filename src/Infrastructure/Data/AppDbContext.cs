using DT_ASPNET.Domain.Notifications;
using DT_ASPNET.Domain.Properties;
using DT_ASPNET.Domain.Reservations;
using DT_ASPNET.Domain.Users;
using DT_ASPNET.Domain.Wishlists;
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