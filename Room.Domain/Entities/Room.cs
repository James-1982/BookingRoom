namespace BookingRoom.Domain.Entities;

public class Room
{
    public int Id { get; private set; }
    public string Name { get; init; }
    public int Capacity { get; init; }
    public List<Booking> Bookings { get; set; } = [];

    public Room() { }

    public Room(string name, int capacity, int? id = null)
    {
        Name = name;
        Capacity = capacity;
        if (id.HasValue)
            Id = id.Value;
    }


    public bool CanBook(Booking newBooking)
        => !Bookings.Any(b => newBooking.ConflictsWith(b));
}
