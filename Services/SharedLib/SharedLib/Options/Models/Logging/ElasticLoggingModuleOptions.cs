using System.ComponentModel.DataAnnotations;

namespace SharedLib.Options.Models.Logging;

public class ElasticLoggingModuleOptions
{
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Elastic Cloud ID length can't be more than 100.")]
    public required string ElasticCloudId { get; init; }

    [Required]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Elastic User length can't be more than 50.")]
    public required string ElasticUser { get; init; }

    [StringLength(50, MinimumLength = 1, ErrorMessage = "Elastic Password length can't be more than 50.")]
    public required string ElasticPassword { get; init; }

    [StringLength(100, MinimumLength = 1, ErrorMessage = "Application Name length can't be more than 100.")]
    public required string ApplicationName { get; init; }

    [StringLength(50, MinimumLength = 1, ErrorMessage = "Application Type length can't be more than 50.")]
    public required string ApplicationType { get; init; }

    [StringLength(100, MinimumLength = 1, ErrorMessage = "Application Namespace length can't be more than 100.")]
    public required string ApplicationNamespace { get; init; }
}