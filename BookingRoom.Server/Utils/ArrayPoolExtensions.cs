using System.Buffers;

namespace BookingRoom.Server.Utils;

internal static class ArrayPoolExtensions
{
    public static void ReturnSafe(this byte[] buffer)
    {
        if (buffer != null) ArrayPool<byte>.Shared.Return(buffer);
    }
}
