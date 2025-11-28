using System.Text.Json.Serialization;
using Core.Converters.JsonConverters;

namespace Core.Models.JiraClient;

public class JiraFields
{
    [JsonPropertyName("created")]
    public DateTime Created { get; set; }
    
    [JsonPropertyName("updated")]
    [JsonConverter(typeof(JiraDateTimeConverter))]
    public DateTime Updated { get; set; }
    
    [JsonPropertyName("resolutiondate")]
    [JsonConverter(typeof(NullableJiraDateTimeConverter))]
    public DateTime? ResolutionDate { get; set; }
    
    [JsonPropertyName("status")]
    public JiraStatus Status { get; set; } = new();
    
    [JsonPropertyName("priority")]
    public JiraPriority Priority { get; set; } = new();
    
    [JsonPropertyName("reporter")]
    public JiraUser Reporter { get; set; } = new();
    
    [JsonPropertyName("assignee")]
    public JiraUser Assignee { get; set; } = new();
    
    [JsonPropertyName("worklog")]
    public JiraWorklogs Worklogs { get; set; } = new();
}