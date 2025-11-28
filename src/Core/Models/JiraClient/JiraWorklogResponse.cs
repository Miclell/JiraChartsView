using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraWorklogResponse
{
    [JsonPropertyName("timeSpentSeconds")]
    public int TimeSpentSeconds { get; set; }
    
    [JsonPropertyName("author")]
    public JiraUser Author { get; set; } = new();
}