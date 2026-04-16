using System.Net;
using System.Net.Sockets;

namespace BookingRoom.Server.Socket;

public class SocketServer(ConnectionPool pool)
{
    private readonly ConnectionPool _pool = pool;
    private readonly System.Net.Sockets.Socket _listenSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly int _port = pool.GetPort();

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _listenSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
        _listenSocket.Listen(1000);

        Console.WriteLine($"Server listening on port {_port}. Press Ctrl+C to stop.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var clientSocket = await _listenSocket.AcceptAsync(cancellationToken);
                var connection = _pool.GetConnection();
                connection.Attach(clientSocket);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }

        Console.WriteLine("Server stopped listening.");
    }
}