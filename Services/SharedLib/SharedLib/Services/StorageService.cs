using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedLib.Options.Models;

namespace SharedLib.Services;
public class StorageService(ILogger<StorageService> logger, IOptionsMonitor<StorageModuleOptions> options) : IStorageService
{
    private readonly StorageModuleOptions _config = options.CurrentValue;

    public async Task<bool> CreateStorageContainer(CancellationToken cancellationToken)
    {
        var blobServiceClient = new BlobServiceClient(_config.ConnectionString);

        logger.LogInformation("Trying to create blob container: {Name}", _config.ContainerName);

        var containerClient = blobServiceClient.GetBlobContainerClient(_config.ContainerName);

        try
        {
            var result = await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            if (result == null)
            {
                logger.LogInformation("Could not create container {Name}. Assuming that container already exists.", _config.ContainerName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not create container");
            return false;
        }

        return true;
    }

    public async Task<bool> UploadFile(Stream file, bool isPublic, CancellationToken cancellationToken, string? fileFormat = "", string? blobNamePostfix = "")
    {
        var blobServiceClient = new BlobServiceClient(_config.ConnectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_config.ContainerName);
        var blobClient = blobContainerClient.GetBlobClient(GetBlobName(_config.BlobName, fileFormat, blobNamePostfix));

        if (isPublic)
            await blobContainerClient.SetAccessPolicyAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var metadata = new Dictionary<string, string>
        {
            { "Author", "FeedBuilderService" },
        };

        try
        {
            await blobClient.UploadAsync(file, overwrite: true, cancellationToken);
            await blobClient.SetMetadataAsync(metadata, cancellationToken: cancellationToken);
            logger.LogInformation("Asset uploaded file with metadata {Metadata}.", metadata);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading file to storage container");
            return false;
        }
    }

    private static string GetBlobName(string baseName, string? fileFormat, string? postfix)
    {
        if (!string.IsNullOrWhiteSpace(postfix))
        {
            baseName = $"{baseName}{postfix}";
        }

        return string.IsNullOrWhiteSpace(fileFormat) ? baseName : $"{baseName}.{fileFormat}";
    }
}
