using Microsoft.Extensions.Hosting;

namespace BookingRoom.Server.Outgoing
{
    public class BookingConsumerHostedService : IHostedService
    {
        private readonly BookingConsumer _consumer;

        public BookingConsumerHostedService()
        {
            _consumer = new BookingConsumer();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(async () => await _consumer.StartAsync(), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // qui potresti chiudere connessione / channel se vuoi
            return Task.CompletedTask;
        }
    }
}