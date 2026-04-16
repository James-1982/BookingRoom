using BookingRoom.Application.Interfaces;
using BookingRoom.Server.BusinessLogic;
using BookingRoom.Server.Services;
using BookingRoom.Server.Socket;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookingRoom.Persistence.PostgreSQL;
using BookingRoom.Persistence.InMemory;

namespace BookingRoom.Server.Utils;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupMapping(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Configure();
        services.AddSingleton(config);
        services.AddSingleton<IMapper, ServiceMapper>();

        return services;
    }

    public static IServiceCollection SetupServices(this IServiceCollection services)
    {
        services.AddSingleton<IBookingService, BookingService>();
        services.AddSingleton<CommandHandler>();
        services.AddSingleton<ConnectionPool>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var persistenceOptions = configuration.GetSection("Persistence").Get<PersistenceOptions>();

        switch (persistenceOptions.Provider)
        {
            case "PostgreSQL":
                services.AddInfrastructure(persistenceOptions.ConnectionStrings.DefaultConnection);
                break;

            default:
                services.AddInfrastructure();
                break;
        }

        return services;
    }
}