using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraWorklogApiResponse
{
    [JsonPropertyName("startAt")]
    public int StartAt { get; set; }
    
    [JsonPropertyName("maxResults")]
    public int MaxResults { get; set; }
    
    [JsonPropertyName("total")]
    public int Total { get; set; }
    
    [JsonPropertyName("worklogs")]
    public List<JiraWorklogItem> Worklogs { get; set; } = new();
}

public class JiraWorklogItem
{
    [JsonPropertyName("timeSpentSeconds")]
    public int TimeSpentSeconds { get; set; }
    
    [JsonPropertyName("timeSpent")]
    public string? TimeSpent { get; set; }
    
    [JsonPropertyName("author")]
    public JiraUser Author { get; set; } = new();
}

