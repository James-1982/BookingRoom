using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BookingRoom.Server.Utils;

public class RabbitMqConsumerTest
{
    public static async Task Start()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "test-queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"📥 Ricevuto: {message}");

            // ACK → diciamo a RabbitMQ che il messaggio è stato processato
            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(
            queue: "test-queue",
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine("👂 In ascolto...");

        // blocca il programma
        await Task.Delay(-1);
    }
}