using System.ComponentModel.DataAnnotations;

namespace SharedLib.Options.Models.Logging;

public class AzureBlobLoggingModuleOptions
{
    public required string ConnectionString { get; init; }

    [StringLength(63, MinimumLength = 0, ErrorMessage = "Azure blob container name cannot be longer than 63 characters.")]
    public required string? ContainerName { get; init; }

    [StringLength(1024, MinimumLength = 0, ErrorMessage = "Azure blob file name cannot be longer than 1024 characters.")]
    public required string? FileName { get; init; } 
}