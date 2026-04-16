using BookingRoom.DTO;
using System.Text.Json;

namespace BookingRoom.Client.WPF.Services;

public interface IBookingService
{
    // ROOMS
    Task<List<RoomDto>> GetRoomsAsync();
    Task<RoomDto?> GetRoomByIdAsync(int id);

    // BOOKINGS
    Task<List<BookingDto>> GetBookingsAsync();
    Task<List<BookingDto>> GetBookingsByRoomAsync(int roomId);

    Task<bool> CreateBookingAsync(BookingDto dto);
    Task<bool> DeleteBookingAsync(int id); //TODO
}

public class BookingService : IBookingService
{
    private readonly BookingClient _client;
    private readonly JsonSerializerOptions _options;

    public BookingService(BookingClient client)
    {
        _client = client;
        _options = new JsonSerializerOptions
        {
            Converters = { new Converter.DateTimeStringConverter() }
        };
    }

    public async Task<List<RoomDto>> GetRoomsAsync()
    {
        try
        {
            string response = await _client.SendRequestAsync(new ActionRequest { Action = BookingAction.GET_ROOMS });
            if (response.StartsWith("ERROR"))
            {
                Console.WriteLine(response);
                return new List<RoomDto>();
            }

            return JsonSerializer.Deserialize<List<RoomDto>>(response, _options) ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return [];
    }

    public async Task<RoomDto?> GetRoomByIdAsync(int id)
    {
        var rooms = await GetRoomsAsync();
        return rooms.FirstOrDefault(r => r.Id == id);
    }

    public async Task<List<BookingDto>> GetBookingsAsync()
    {
        try
        {
            string response = await _client.SendRequestAsync(new ActionRequest { Action = BookingAction.GET_BOOKINGS });
            if (response.StartsWith("ERROR"))
            {
                Console.WriteLine(response);
                return [];
            }

            var allBookings = JsonSerializer.Deserialize<List<BookingDto>>(response, _options) ?? [];

            return [.. allBookings.OrderBy(b => b.StartTime)];

        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex.Message);
        }

        return [];
    }

    public async Task<List<BookingDto>> GetBookingsByRoomAsync(int roomId)
    {
        var bookings = await GetBookingsAsync();
        return [.. bookings.Where(b => b.RoomId == roomId)];
    }

    public async Task<bool> CreateBookingAsync(BookingDto dto)
    {
        var request = new BookingRequest
        {
            Action = BookingAction.CREATE_BOOKING,
            Payload = dto
        };

        string response = await _client.SendRequestAsync(request);

        if (!response.StartsWith("ERROR"))
        {
            return true;
        }
        else
        {
            Console.WriteLine(response);
            return false;
        }
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        // TODO: Implement delete booking logic
        return false;
    }
}