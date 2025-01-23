using System.ComponentModel.DataAnnotations;

namespace SharedLib.Options.Models;
public class StorageModuleOptions
{
    [MinLength(1)]
    public required string ConnectionString { get; init; }

    [MinLength(1)]
    public required string ContainerName { get; init; }

    [MinLength(1)]
    public required string BlobName { get; init; }

}
