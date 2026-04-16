using BookingRoom.Application.Interfaces;
using BookingRoom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Persistence.PostgreSQL;

public class BookingsRepository(BookingDbContext context) : IBookingRepository
{
    private readonly BookingDbContext _context = context;

    public IEnumerable<Booking> GetBookings()
    {
        return [.. _context.Bookings.AsNoTracking()];
    }

    // Ottieni booking filtrando per roomId
    public IEnumerable<Booking> GetBookingsByRoom(int roomId)
    {
        return [.. _context.Bookings
                       .AsNoTracking()
                       .Where(b => b.RoomId == roomId)];
    }

    // Aggiungi un nuovo booking
    public void AddBooking(Booking booking)
    {
        _context.Bookings.Add(booking);
        _context.SaveChanges();
    }
}