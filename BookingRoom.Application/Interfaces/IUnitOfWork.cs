namespace BookingRoom.Application.Interfaces;

public interface IUnitOfWork
{
    IRoomRepository Rooms { get; }
    IBookingRepository Bookings { get; }
    void Commit();
}
