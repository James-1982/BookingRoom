using System.Net.Sockets;
using System.Text;
using System.Text.Json;

public class Client(string serverIp, int port) 
{
    private readonly string _serverIp = serverIp;
    private readonly int _port = port;

    public void SendRequest<T>(T request)
    {
        try
        {
            using TcpClient client = new TcpClient(_serverIp, _port);
            NetworkStream stream = client.GetStream();

            // 1️⃣ Serializza l'oggetto in JSON
            var jsonOptions = new JsonSerializerOptions { WriteIndented = false };
            string json = JsonSerializer.Serialize(request, jsonOptions);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            // 2️⃣ Prepend 4 byte di lunghezza
            byte[] lengthPrefix = BitConverter.GetBytes(jsonBytes.Length);
            byte[] packet = new byte[lengthPrefix.Length + jsonBytes.Length];
            Array.Copy(lengthPrefix, 0, packet, 0, 4);
            Array.Copy(jsonBytes, 0, packet, 4, jsonBytes.Length);

            // 3️⃣ Invia al server
            stream.Write(packet, 0, packet.Length);

            // Riceve risposta
            byte[] buffer = new byte[4096];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine("----- RISPOSTA SERVER -----");
            Console.WriteLine(response);
            Console.WriteLine("---------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore: {ex.Message}");
        }
    }
}