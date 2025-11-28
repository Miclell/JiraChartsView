using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraWorklogs
{
    [JsonPropertyName("worklogs")]
    public List<JiraWorklogResponse> Worklogs { get; set; } = new();
}