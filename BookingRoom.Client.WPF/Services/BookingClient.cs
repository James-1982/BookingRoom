using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace BookingRoom.Client.WPF.Services;

public class BookingClient(string serverIp, int port)
{
    private readonly string _serverIp = serverIp;
    private readonly int _port = port;
    public async Task<string> SendRequestAsync<T>(T request)
    {
        try
        {
            using TcpClient client = new TcpClient();
            await client.ConnectAsync(_serverIp, _port);

            using NetworkStream stream = client.GetStream();

            // 1. Serialize
            var jsonOptions = new JsonSerializerOptions { WriteIndented = false };
            string json = JsonSerializer.Serialize(request, jsonOptions);
            byte[] payload = Encoding.UTF8.GetBytes(json);

            // 2. Prefix length (4 bytes)
            byte[] lengthPrefix = BitConverter.GetBytes(payload.Length);

            // 3. Send full packet
            await stream.WriteAsync(lengthPrefix);
            await stream.WriteAsync(payload);

            // 4. Read response length (IMPORTANTISSIMO)
            byte[] lengthBuffer = await ReadExactAsync(stream, 4);

            int responseLength = BitConverter.ToInt32(lengthBuffer, 0);

            if (responseLength <= 0)
                return "ERROR: invalid response length";

            // 5. Read full response
            byte[] responseBuffer = await ReadExactAsync(stream, responseLength);

            return Encoding.UTF8.GetString(responseBuffer);
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }

    private static async Task<byte[]> ReadExactAsync(NetworkStream stream, int size)
    {
        byte[] buffer = new byte[size];
        int offset = 0;

        while (offset < size)
        {
            int read = await stream.ReadAsync(buffer, offset, size - offset);

            if (read == 0)
                throw new Exception("Connection closed unexpectedly");

            offset += read;
        }

        return buffer;
    }
}