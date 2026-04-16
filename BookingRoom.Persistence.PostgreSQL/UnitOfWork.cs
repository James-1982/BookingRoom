using BookingRoom.Application.Interfaces;

namespace BookingRoom.Persistence.PostgreSQL;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly BookingDbContext _context;

    public IRoomRepository Rooms { get; }
    public IBookingRepository Bookings { get; }

    public UnitOfWork(BookingDbContext context)
    {
        _context = context;
        Rooms = new RoomRepository(_context);
        Bookings = new BookingsRepository(_context);
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
