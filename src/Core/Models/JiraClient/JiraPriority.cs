using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraPriority
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}