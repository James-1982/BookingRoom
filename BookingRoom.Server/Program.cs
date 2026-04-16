using BookingRoom.Application.Interfaces;
using BookingRoom.Server.Outgoing;
using BookingRoom.Server.Socket;
using BookingRoom.Server.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                         .Build();

        using IHost host = Host.CreateDefaultBuilder(args)
                            .ConfigureServices((context, services) =>
                            {
                                services.Configure<ServerOptions>(configuration.GetSection("ServerOptions"));
                                services.Configure<PersistenceOptions>(configuration.GetSection("Persistence"));

                                services.AddInfrastructure(configuration);

                                services.SetupMapping();
                                services.SetupServices();

                                // SocketServer e HostedService
                                services.AddSingleton<SocketServer>();
                                services.AddHostedService<ServerHostedService>();
                                services.AddHostedService<BookingConsumerHostedService>();
                            })
                            .Build();

        Console.WriteLine("Server starting...");

        using (var scope = host.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await initializer.InitializeAsync();
        }

        await host.RunAsync();

        Console.WriteLine("Server stopped.");
    }
}