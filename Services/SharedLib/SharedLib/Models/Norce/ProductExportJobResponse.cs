using System.Text.Json.Serialization;

namespace SharedLib.Models.Norce;

public class ProductExportJobResponse
{
    [JsonPropertyName("clientId")]
    public int ClientId { get; set; }
    
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
    
    [JsonPropertyName("channelKey")]
    public string ChannelKey { get; set; } = string.Empty;

    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("deltaFromDate")]
    public DateTime? DeltaFromDate { get; set; }
    
    [JsonPropertyName("itemsTotal")]
    public int ItemsTotal { get; set; }
    
    [JsonPropertyName("statusId")]
    public string StatusId { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public DateTime Start { get; set; }
    
    [JsonPropertyName("stop")]
    public DateTime? Stop { get; set; }
    
    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; }
}