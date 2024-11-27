using System.ComponentModel.DataAnnotations;

namespace SharedLib.Options.Models;

public class NorceBaseModuleOptions
{
    [MinLength(1)] public required string ClientId { get; init; }
    [MinLength(1)] public required string ClientSecret { get; init; }
    [MinLength(1)] public required string Environment { get; init; }
    [MinLength(1)] public required int ApplicationId { get; init; }
    [MinLength(1)] public required string BaseUrl { get; init; }
    public required int UserId { get; init; }
    public string CdnUrl { get; init; } = string.Empty;
}
