using RabbitMQ.Client;
using System.Text;

namespace BookingRoom.Server.Utils;

public class RabbitMqTest
{
    public static async Task Test()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        // 1. Crea una queue (se non esiste)
        await channel.QueueDeclareAsync(
            queue: "test-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // 2. Messaggio
        var message = "Hello RabbitMQ!";
        var body = Encoding.UTF8.GetBytes(message);

        // 3. Pubblica
        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: "test-queue",
            body: body
        );

        Console.WriteLine("📤 Messaggio inviato!");
    }
}
