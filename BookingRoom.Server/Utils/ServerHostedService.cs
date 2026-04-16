using BookingRoom.Server.Socket;
using Microsoft.Extensions.Hosting;

namespace BookingRoom.Server.Utils;

public class ServerHostedService(SocketServer server) : IHostedService
{
    private readonly SocketServer _server = server;
    private readonly CancellationTokenSource _cts = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = _server.StartAsync(_cts.Token);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();

        return Task.CompletedTask;
    }
}
