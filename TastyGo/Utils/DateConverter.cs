using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

public class DateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format = "yyyy-MM-ddTHH:mm:ss.fffZ"; // Adjust as needed

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
        {
            return date;
        }

        throw new JsonException($"Invalid date format: {value}");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToUniversalTime().ToString(_format, CultureInfo.InvariantCulture));
    }
}
