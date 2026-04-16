
namespace BookingRoom.Server.Utils;

public class ServerOptions
{
    public int Port { get; set; } = 5000;
    public int MaxConnections { get; set; } = 1000;
    public int BufferSize { get; set; } = 8192;
}
