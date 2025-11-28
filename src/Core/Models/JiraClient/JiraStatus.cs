using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraStatus
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("statusCategory")]
    public JiraStatusCategory StatusCategory { get; set; } = new();
}