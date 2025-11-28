using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraIssue
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
    
    [JsonPropertyName("fields")]
    public JiraFields Fields { get; set; } = new();
}