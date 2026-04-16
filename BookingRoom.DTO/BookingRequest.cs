namespace BookingRoom.DTO;

public class ActionRequest
{
    public string Action { get; set; } = string.Empty;
}

public class BookingRequest : ActionRequest
{
    public BookingDto? Payload { get; set; }
}

public class BookingDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class RoomDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int Capacity { get; init; }
}