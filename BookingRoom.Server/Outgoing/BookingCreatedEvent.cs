namespace BookingRoom.Server.Outgoing;

public class BookingCreatedEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public int RoomId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string ClientName { get; set; } = "";
}
