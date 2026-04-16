using BookingRoom.Server.BusinessLogic;
using BookingRoom.Server.Utils;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace BookingRoom.Server.Socket;

public class ConnectionPool
{
    private readonly ConcurrentBag<Connection> _connections = [];
    private readonly CommandHandler _handler;
    private readonly ServerOptions _options;
    private readonly int _timeoutSeconds = 10;
    private readonly Timer _timer;

    public ConnectionPool(CommandHandler handler, IOptions<ServerOptions> options)
    {
        _handler = handler;
        _options = options.Value;

        for (int i = 0; i < _options.MaxConnections; i++)
            _connections.Add(new Connection(_options.BufferSize, _handler));

        _timer = new Timer(CheckTimeouts, null, 5000, 5000);
    }

    public int GetPort() => _options.Port;

    public Connection GetConnection() => _connections.TryTake(out var c) ? c : new Connection(_options.BufferSize, _handler);

    public void AddConnection(Connection conn)
    {
        _connections.Add(conn);
    }

    public void RemoveConnection(Connection conn)
    {
        _connections.TryTake(out conn);
    }

    private void CheckTimeouts(object? state)
    {
        var now = DateTime.UtcNow;

        var activeConnections = _connections.ToArray();

        var expiredConnections = activeConnections
          .Where(conn => conn.Socket != null && (now - conn.LastActivity).TotalSeconds > _timeoutSeconds)
          .ToArray();

        Parallel.ForEach(expiredConnections, conn =>
        {
            Console.WriteLine($"Closing inactive client (>{_timeoutSeconds}s)");
            conn.CloseConnection();
            RemoveConnection(conn);
        });
    }

    public Connection[] GetActiveConnections()
    {
        return [.. _connections];
    }

    public Connection GetConnection(int bufferSize = 8192)
    {
        foreach (var conn in _connections)
        {
            if (!conn.Socket.Connected)
            {
                RemoveConnection(conn);
                return conn;
            }
        }

        var newConn = new Connection(bufferSize, _handler);
        AddConnection(newConn);
        return newConn;
    }
}