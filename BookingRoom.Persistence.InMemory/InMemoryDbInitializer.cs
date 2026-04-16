using BookingRoom.Application.Interfaces;
using BookingRoom.Domain.Entities;

namespace BookingRoom.Persistence.InMemory;

public class InMemoryDbInitializer(InMemoryContext context) : IDbInitializer
{
    private readonly InMemoryContext _context = context;

    public Task InitializeAsync()
    {
        if (!_context.Rooms.Any())
        {
            _context.Rooms.AddRange(
                new Room("Sala A", 15, 1),
                new Room("Sala B", 25, 2)
            );
        }

        return Task.CompletedTask;
    }
}