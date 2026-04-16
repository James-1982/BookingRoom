using BookingRoom.Domain.Entities;
using BookingRoom.DTO;
using Mapster;

namespace BookingRoom.Server.Utils;

public static class MappingConfig
{
    public static void Configure()
    {
        // Room -> RoomDto
        TypeAdapterConfig<Room, RoomDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Capacity, src => src.Capacity)
            // Non mappare i booking qui se vuoi solo le stanze
            .IgnoreNullValues(true);

        // Booking -> BookingDto
        TypeAdapterConfig<Booking, BookingDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.RoomId, src => src.RoomId)
            .Map(dest => dest.StartTime, src => src.StartTime)
            .Map(dest => dest.EndTime, src => src.EndTime);
    }
}