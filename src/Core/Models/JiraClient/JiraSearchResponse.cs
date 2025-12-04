using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraSearchResponse
{
    [JsonPropertyName("startAt")]
    public int StartAt { get; set; }

    [JsonPropertyName("maxResults")]
    public int MaxResults { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("issues")]
    public List<JiraIssue> Issues { get; set; } = new();
}