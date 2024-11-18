namespace FeedService.Domain.DataFeedWatch;

public class GenericFeedProductDto
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? ProductLink { get; set; }
    public string? ImageLink { get; set; }
    public List<string?> AdditionalImages { get; set; } = [];
    public required string Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? Availability { get; set; }
    public string? Category { get; set; }
    public string? EanCode { get; set; }
    public Dictionary<string, string>? Flags { get; set; }
    public Dictionary<string, string>? Parametrics { get; set; }
}