using BookingRoom.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BookingRoom.Persistence.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryContext>();
        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
        services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();
        services.AddScoped<IDbInitializer, InMemoryDbInitializer>();

        return services;
    }
}
