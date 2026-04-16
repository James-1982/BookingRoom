using BookingRoom.Application.Interfaces;
using BookingRoom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Persistence.PostgreSQL;

public class RoomRepository(BookingDbContext context) : IRoomRepository
{
    private readonly BookingDbContext _context = context;

    public IEnumerable<Room> GetRooms()
    {
        return [.. _context.Rooms.AsNoTracking()];
    }

    public Room? GetRoomById(int id)
    {
        return _context.Rooms
                       .AsNoTracking()
                       .FirstOrDefault(r => r.Id == id);
    }

    public IEnumerable<Room> GetRoomsWithBookings()
    {
        return [.. _context.Rooms
                       .Include(r => r.Bookings)
                       .AsNoTracking()];
    }

    public Room? GetRoomWithBookingsById(int id)
    {
        return _context.Rooms
                       .Include(r => r.Bookings)
                       .AsNoTracking()
                       .FirstOrDefault(r => r.Id == id);
    }
}
