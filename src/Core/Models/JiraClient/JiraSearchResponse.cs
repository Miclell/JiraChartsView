using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraSearchResponse
{
    [JsonPropertyName("issues")]
    public List<JiraIssue> Issues { get; set; } = new();
}