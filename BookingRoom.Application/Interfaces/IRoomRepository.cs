using BookingRoom.Domain.Entities;

namespace BookingRoom.Application.Interfaces;

public interface IRoomRepository
{
    IEnumerable<Room> GetRooms();

    Room? GetRoomById(int id);

    IEnumerable<Room> GetRoomsWithBookings();

    Room? GetRoomWithBookingsById(int id);
}
