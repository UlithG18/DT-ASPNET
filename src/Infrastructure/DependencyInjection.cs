using DT_ASPNET.Application.Notifications;
using DT_ASPNET.Application.Properties;
using DT_ASPNET.Application.Reservations;
using DT_ASPNET.Application.Users;
using DT_ASPNET.Application.Wishlists;
using DT_ASPNET.Domain.Notifications;
using DT_ASPNET.Domain.Properties;
using DT_ASPNET.Domain.Reservations;
using DT_ASPNET.Domain.Users;
using DT_ASPNET.Domain.Wishlists;
using DT_ASPNET.Infrastructure.Data;
using DT_ASPNET.Infrastructure.Kyc;
using DT_ASPNET.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DT_ASPNET.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        // Base de datos
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("Postgres")));

        // Repositorios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // KYC AI provider
        services.AddScoped<IKycAiProvider, AiKycProvider>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IKycService, KycService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}