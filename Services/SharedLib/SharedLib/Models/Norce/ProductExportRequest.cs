using System.Text.Json.Serialization;

namespace SharedLib.Models.Norce;

public class ProductExportRequest
{
    [JsonPropertyName("channelKey")]
    public string ChannelKey { get; set; } = string.Empty;

    [JsonPropertyName("deltaFromDate")]
    public string? DeltaFromDate { get; set; }
}