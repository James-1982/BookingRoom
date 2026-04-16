using BookingRoom.Application.Interfaces;
using BookingRoom.Domain.Entities;
using BookingRoom.DTO;
using BookingRoom.Server.Outgoing;
using BookingRoom.Server.Socket;
using BookingRoom.Server.Utils;
using MapsterMapper;
using System.Text.Json;

namespace BookingRoom.Server.BusinessLogic;

public class CommandHandler
{
    private readonly JsonSerializerOptions _options;

    private readonly IBookingService? _bookingService;

    private readonly IMapper? _mapper;

    private readonly EventBus _eventBus = new EventBus();

    public CommandHandler(IBookingService bookingService, IMapper mapper)
    {
        _bookingService = bookingService;
        _mapper = mapper;
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new DateTimeStringConverter());
    }

    public async Task HandleMessage(Connection conn, byte[] payload)
    {
        if (payload.Length == 0)
        {
            conn.EnqueueSend(BuildResponse("ERROR: Empty payload"));
            return;
        }

        ActionRequest? actionReq;

        try
        {
            actionReq = JsonSerializer.Deserialize<ActionRequest>(payload);
        }
        catch (JsonException)
        {
            conn.EnqueueSend(BuildResponse("ERROR: Invalid JSON"));
            return;
        }

        if (actionReq == null || string.IsNullOrWhiteSpace(actionReq.Action))
        {
            conn.EnqueueSend(BuildResponse("ERROR: Invalid request format"));
            return;
        }

        byte[] response = actionReq.Action.ToUpperInvariant() switch
        {
            BookingAction.GET_ROOMS => BuildResponse(_mapper.Map<List<RoomDto>>(_bookingService.GetRooms())),

            BookingAction.GET_BOOKINGS => BuildResponse(_mapper.Map<List<BookingDto>>(_bookingService.GetBookings())),

            BookingAction.CREATE_BOOKING => await HandleCreateBooking(payload),

            _ => BuildResponse($"ERROR: Unknown action {actionReq.Action}"),
        };
        conn.EnqueueSend(response);
    }

    private async Task<byte[]> HandleCreateBooking(byte[] payload)
    {
        BookingRequest? createReq;
        try
        {
            createReq = JsonSerializer.Deserialize<BookingRequest>(payload, _options);

        }
        catch (JsonException)
        {
            return BuildResponse("ERROR: Invalid CREATE_BOOKING payload");
        }

        if (createReq.Payload == null)
            return BuildResponse("ERROR: Missing payload");

        var booking = _mapper.Map<Booking>(createReq.Payload);

        if (booking == null)
            return BuildResponse("ERROR: Invalid payload");

        if (!booking.IsValid(out var error))
        {
            Console.WriteLine(error);
            return BuildResponse($"ERROR: {error}");
        }

        var result = _bookingService.TryCreateBooking(booking, out var err);

        if (result)
            await PublishEvent(booking);

        return result
            ? BuildResponse($"OK: Booking created")
            : BuildResponse($"ERROR: {err}");
    }

    private byte[] BuildResponse(object data)
    {
        string json = JsonSerializer.Serialize(data, _options);
        byte[] payload = System.Text.Encoding.UTF8.GetBytes(json);
        byte[] packet = new byte[4 + payload.Length];
        Array.Copy(BitConverter.GetBytes(payload.Length), packet, 4);
        Array.Copy(payload, 0, packet, 4, payload.Length);
        return packet;
    }

    private async Task PublishEvent(Booking booking)
    {
         var evt = new BookingCreatedEvent
         {
             RoomId = booking.RoomId,
             Start = booking.StartTime,
             End = booking.EndTime,
         };

        await _eventBus.PublishBookingCreatedAsync(evt);
    }
}