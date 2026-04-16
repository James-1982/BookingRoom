using BookingRoom.DTO;
using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        Client client = new("127.0.0.1", 5000);

        while (true)
        {
            Console.WriteLine($"Comandi disponibili: {BookingAction.GET_ROOMS}, {BookingAction.GET_BOOKINGS}, {BookingAction.CREATE_BOOKING}, EXIT");
            Console.Write("Inserisci comando: ");

            string command = Console.ReadLine().Trim().ToUpper();

            if (command == "EXIT") break;

            switch (command)
            {
                case BookingAction.GET_ROOMS:
                case BookingAction.GET_BOOKINGS:
                    {
                        client.SendRequest(new ActionRequest { Action = command });
                        break;
                    }

                case BookingAction.CREATE_BOOKING:
                    {
                        var booking = ReadBookingFromConsole();

                        var request = new BookingRequest
                        {
                            Action = command,
                            Payload = booking
                        };

                        client.SendRequest(request);
                        break;
                    }

                default:
                    {
                        Console.WriteLine("Comando non valido");
                        break;
                    }
            }
        }
    }

    private static BookingDto ReadBookingFromConsole()
    {
        var booking = new BookingDto
        {
            Id = Random.Shared.Next(1000, 9999)
        };

        Console.Write("Titolo: ");
        booking.Title = Console.ReadLine();

        Console.Write("RoomId: ");
        booking.RoomId = int.Parse(Console.ReadLine());

        Console.Write("StartTime (yyyy-MM-dd HH:mm): ");
        booking.StartTime = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

        Console.Write("EndTime (yyyy-MM-dd HH:mm): ");
        booking.EndTime = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

        return booking;
    }
}