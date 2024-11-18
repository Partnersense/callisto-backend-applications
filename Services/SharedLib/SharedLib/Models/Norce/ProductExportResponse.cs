using System.Text.Json.Serialization;

namespace SharedLib.Models.Norce;

public class ProductExportResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
  
    [JsonPropertyName("jobKey")]
    public string? JobKey { get; set; }
  
    [JsonPropertyName("dataUrl")]
    public string? DataUrl { get; set; }
   
    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
  
    [JsonPropertyName("itemsTotal")]
    public int ItemsTotal { get; set; }
}