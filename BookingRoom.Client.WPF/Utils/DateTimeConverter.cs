using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookingRoom.Client.WPF.Utils;

public class DateTimeStringConverter : JsonConverter<DateTime>
{
    private readonly string _format = "yyyy-MM-dd HH:mm";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? s = reader.GetString();
        if (string.IsNullOrWhiteSpace(s))
            throw new JsonException("Invalid datetime");

        var localDate = DateTime.Parse(s, CultureInfo.InvariantCulture);

        return localDate.ToLocalTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var localDate = value.ToLocalTime();
        writer.WriteStringValue(localDate.ToString(_format));
    }
}
