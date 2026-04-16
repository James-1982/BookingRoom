using BookingRoom.Application.Interfaces;
using BookingRoom.Domain.Entities;

namespace BookingRoom.Persistence.InMemory;

public class InMemoryBookingRepository(InMemoryContext contex) : IBookingRepository
{
    private readonly InMemoryContext _contex = contex;

    public IEnumerable<Booking> GetBookings() => _contex.Bookings;

    public void AddBooking(Booking booking)
    {
        if (!_contex.Rooms.Any(r => r.Id == booking.RoomId))
            throw new InvalidOperationException("Room does not exist");

        _contex.Bookings.Add(booking);
    }

    public IEnumerable<Booking> GetBookingsByRoom(int roomId)
    {
        return _contex.Bookings.Where(b => b.RoomId == roomId);
    }
}