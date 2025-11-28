using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraStatusCategory
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
}