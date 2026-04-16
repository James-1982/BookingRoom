using BookingRoom.Domain.Entities;

namespace BookingRoom.Application.Interfaces;

public interface IBookingService
{
    IEnumerable<Room> GetRooms();
    IEnumerable<Booking> GetBookings();

    /// <summary>
    /// Tenta di creare una prenotazione.
    /// Restituisce true se ok, false se conflitto
    /// </summary>
    bool TryCreateBooking(Booking booking, out string? errorMessage);
}
