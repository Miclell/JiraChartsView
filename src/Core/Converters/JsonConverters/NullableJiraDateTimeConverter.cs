using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Converters.JsonConverters;

public class NullableJiraDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.String) return null;
        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
            return null;

        if (DateTime.TryParse(dateString, out var date)) return date;

        if (DateTime.TryParseExact(dateString,
                "yyyy-MM-ddTHH:mm:ss.fffzzz",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out var jiraDate))
            return jiraDate;

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
        else
            writer.WriteNullValue();
    }
}