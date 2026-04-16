using BookingRoom.Domain.Entities;

namespace BookingRoom.Persistence.InMemory;

public class InMemoryContext
{
    public List<Room> Rooms { get; } = [];
    public List<Booking> Bookings { get; } = [];

    public InMemoryContext()
    {
    }
}
