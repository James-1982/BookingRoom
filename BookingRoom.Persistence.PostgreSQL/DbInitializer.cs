using BookingRoom.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Persistence.PostgreSQL;

public class DbInitializer(BookingDbContext context) : IDbInitializer
{
    private readonly BookingDbContext _context = context;
    public async Task InitializeAsync()
    {
        await _context.Database.MigrateAsync();

        if (!_context.Rooms.Any())
        {
            _context.Rooms.AddRange(
                new Domain.Entities.Room("Andromeda", 15),
                new Domain.Entities.Room("Basileia",  25),
                new Domain.Entities.Room("Cassiopea", 35)
            );

            await context.SaveChangesAsync();
        }
    }
}