using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Converters.JsonConverters;

public class JiraDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String) return DateTime.MinValue;
        var dateString = reader.GetString();
        if (DateTime.TryParse(dateString, out var date)) return date;

        return DateTime.TryParseExact(dateString,
            "yyyy-MM-ddTHH:mm:ss.fffzzz",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out var jiraDate)
            ? jiraDate
            : DateTime.MinValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
    }
}