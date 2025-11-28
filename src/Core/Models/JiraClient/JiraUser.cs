using System.Text.Json.Serialization;

namespace Core.Models.JiraClient;

public class JiraUser
{
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;
    
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; } = string.Empty;
    
    [JsonPropertyName("emailAddress")]
    public string EmailAddress { get; set; } = string.Empty;
    
    [JsonPropertyName("active")]
    public bool Active { get; set; }
}