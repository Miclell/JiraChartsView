using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraChangeItem
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;
    
    [JsonPropertyName("fromString")]
    public string FromString { get; set; } = string.Empty;
    
#pragma warning disable CS0108
    [JsonPropertyName("toString")]
    public string ToString { get; set; } = string.Empty;
#pragma warning restore CS0108
    
    [JsonPropertyName("fieldtype")]
    public string FieldType { get; set; } = string.Empty;
}