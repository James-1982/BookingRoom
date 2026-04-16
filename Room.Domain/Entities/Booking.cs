namespace BookingRoom.Domain.Entities;

public class Booking
{
    public int Id { get; private set; }
    public string Title { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public Room? Room { get; set; }

    public Booking() { }

    public Booking(string title, int roomId, DateTime start, DateTime end, int? id = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title required", nameof(title));

        if (start >= end)
            throw new ArgumentException("Start must be before End");

        Title = title;
        RoomId = roomId;
        StartTime = start;
        EndTime = end;

        if (id.HasValue)
            Id = id.Value;
    }

    public bool IsValid(out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            errorMessage = "Title is required";
            return false;
        }

        if (StartTime >= EndTime)
        {
            errorMessage = "StartTime must be before EndTime";
            return false;
        }

        errorMessage = null;
        return true;
    }

    public bool ConflictsWith(Booking other)
        => StartTime < other.EndTime && EndTime > other.StartTime;

}