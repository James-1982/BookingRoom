namespace BookingRoom.Server.Utils;

public class PersistenceOptions
{
    public string Provider { get; set; } = "InMemory"; // default
    public ConnectionStringsOptions ConnectionStrings { get; set; } = new();
}

public class ConnectionStringsOptions
{
    public string DefaultConnection { get; set; } = string.Empty;
}
