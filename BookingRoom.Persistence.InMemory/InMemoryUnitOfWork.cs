using BookingRoom.Application.Interfaces;

namespace BookingRoom.Persistence.InMemory;

public class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly InMemoryContext _context;

    public InMemoryUnitOfWork(InMemoryContext context)
    {
        _context = context;
    }

    public IRoomRepository Rooms => new InMemoryRoomRepository(_context);
    public IBookingRepository Bookings => new InMemoryBookingRepository(_context);

    public void Commit()
    {
    }
}
