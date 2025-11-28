using System.Text.Json.Serialization;
using Core.Converters.JsonConverters;

namespace Core.Models.JiraClient;

public class JiraHistory
{
    [JsonPropertyName("created")]
    [JsonConverter(typeof(JiraDateTimeConverter))]
    public DateTime Created { get; set; }
    
    [JsonPropertyName("items")]
    public List<JiraChangeItem> Items { get; set; } = new();
}