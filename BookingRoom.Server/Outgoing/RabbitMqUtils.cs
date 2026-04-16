using RabbitMQ.Client;
using System;

namespace BookingRoom.Server.Outgoing;

public static class RabbitMqUtils
{
    public static async Task<bool> IsRabbitRunning(string hostName = "localhost", int port = 5672)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                RequestedConnectionTimeout = TimeSpan.FromMilliseconds(500)
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            return true; // connesso con successo
        }
        catch
        {
            return false; // Rabbit non disponibile
        }
    }
}