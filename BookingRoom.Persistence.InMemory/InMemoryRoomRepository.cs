using BookingRoom.Application.Interfaces;
using BookingRoom.Domain.Entities;

namespace BookingRoom.Persistence.InMemory;

public class InMemoryRoomRepository(InMemoryContext context) : IRoomRepository
{
    private readonly InMemoryContext _context = context;

    public IEnumerable<Room> GetRooms()
    {
        return _context.Rooms;
    }

    public Room? GetRoomById(int id)
    {
        return _context.Rooms.FirstOrDefault(r => r.Id == id);
    }

    public IEnumerable<Room> GetRoomsWithBookings()
    {
        return _context.Rooms.Select(r => new Room(r.Name, r.Capacity, r.Id)
        {
            Bookings = [.. _context.Bookings.Where(b => b.RoomId == r.Id)]
        });
    }

    public Room? GetRoomWithBookingsById(int id)
    {
        var r = _context.Rooms.FirstOrDefault(r => r.Id == id);
        if (r == null) return null;

        return new Room(r.Name, r.Capacity, r.Id)
        {
            Bookings = [.. _context.Bookings.Where(b => b.RoomId == r.Id)]
        };
    }
}
