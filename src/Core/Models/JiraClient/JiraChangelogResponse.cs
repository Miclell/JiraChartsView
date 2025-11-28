using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraChangelogResponse
{
    [JsonPropertyName("histories")]
    public List<JiraHistory> Histories { get; set; } = new();
}