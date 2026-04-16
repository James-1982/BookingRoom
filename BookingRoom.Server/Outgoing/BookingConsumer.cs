using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BookingRoom.Server.Outgoing;

public class BookingConsumer
{
    private readonly ConnectionFactory _factory;

    public BookingConsumer()
    {
        _factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
    }

    public async Task StartAsync()
    {
        var connection = await _factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "booking-created",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var evt = JsonSerializer.Deserialize<BookingCreatedEvent>(message);

                // --- Qui metti la logica reale ---
                if (evt != null)
                {
                    Console.WriteLine($"✅ Booking {evt.Id} creato per Room {evt.RoomId}");

                    // Esempio: invia email
                    // await _notificationService.SendBookingCreatedEmail(evt);

                    // Esempio: aggiorna cache / dashboard
                    // _dashboardService.AddBooking(evt);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ Errore deserializzazione evento: {ex.Message}");
            }

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(
            queue: "booking-created",
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine("👂 Consumer reale attivo, in ascolto...");
        await Task.Delay(-1);
    }
}