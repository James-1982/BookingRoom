using BookingRoom.DTO;

namespace BookingRoom.Client.WPF.Utils
{
    public static class Extensions
    {
        public static bool IsExpired(this BookingDto booking)
            => booking.EndTime < DateTime.Now;

        public static bool IsRunning(this BookingDto booking)
            => booking.StartTime <= DateTime.Now && booking.EndTime > DateTime.Now;
    }
}
