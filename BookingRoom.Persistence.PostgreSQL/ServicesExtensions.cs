using BookingRoom.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookingRoom.Persistence.PostgreSQL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BookingDbContext>(options =>
        options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBookingRepository, BookingsRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}