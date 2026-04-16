using BookingRoom.Domain.Entities;

namespace BookingRoom.Application.Interfaces;

public interface IBookingRepository
{
    IEnumerable<Booking> GetBookings();
    IEnumerable<Booking> GetBookingsByRoom(int roomId);
    void AddBooking(Booking booking);
}