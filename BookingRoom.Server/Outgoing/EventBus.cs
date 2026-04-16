using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BookingRoom.Server.Outgoing;

public class EventBus
{
    private const int MaxRetryAttempts = 3;
    private readonly ConnectionFactory _factory;

    public EventBus()
    {
        _factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
    }

    public async Task PublishBookingCreatedAsync(BookingCreatedEvent evt)
    {
        int retry = MaxRetryAttempts;

        if (!await RabbitMqUtils.IsRabbitRunning())
        {
            Console.WriteLine("RabbitMQ non disponibile, evento ignorato.");
            return;
        }

        while (retry > 0)
        {
            try
            {
                using var connection = await _factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    queue: "booking-created",
                    durable: true, // rendi persistente in produzione
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(evt));

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "booking-created",
                    body: body
                );

                Console.WriteLine($"📤 Evento Booking pubblicato: {evt.Id}");

                break; // successo
            }
            catch (Exception ex)
            {
                retry--;
                Console.WriteLine($"❌ Errore pubblicazione evento ({MaxRetryAttempts - retry}/{MaxRetryAttempts}): {ex.Message}");
                if (retry > 0)
                    await Task.Delay(500); // attesa 0,5s prima del retry
                else throw; // dopo 3 tentativi fallo crashare/logga
            }
        }
    }
}