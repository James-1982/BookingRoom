using BookingRoom.Application.Interfaces;
using BookingRoom.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace BookingRoom.Server.Services;

public class BookingService(IServiceProvider provider) : IBookingService
{
    private readonly IServiceProvider _provider = provider;

    public IEnumerable<Room> GetRooms()
    {
        using var scope = _provider.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        return uow.Rooms.GetRooms();
    }

    public IEnumerable<Booking> GetBookings()
    {
        using var scope = _provider.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        return uow.Bookings.GetBookings();
    }

    public bool TryCreateBooking(Booking booking, out string? errorMessage)
    {
        using var scope = _provider.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Ottieni la room con tutti i booking correnti
        var room = uow.Rooms.GetRoomWithBookingsById(booking.RoomId);
        if (room == null)
        {
            errorMessage = "Room does not exist";
            return false;
        }

        // Usa il metodo di Room solo per validazione
        if (!room.CanBook(booking))
        {
            errorMessage = "Room already booked in this time slot";
            return false;
        }

        // Aggiungi la booking al repository reale
        uow.Bookings.AddBooking(booking);
        uow.Commit();

        errorMessage = null;
        return true;
    }
}