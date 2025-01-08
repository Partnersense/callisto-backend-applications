using System.Text.Json;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using FeedService.Domain.Internal.MethodOptions;
using Microsoft.Extensions.Options;
using SharedLib.Logging.Enums;
using SharedLib.Options.Models;
using SharedLib.Services;

namespace FeedService.Services.StorageServices
{
    /// <summary>
    /// Azure Blob Storage implementation of IStorageUploadService
    /// </summary>
    public class AzureStorageService(
        ILogger<AzureStorageService> logger,
        IStorageService storageService)
        : IStorageUploadService
    {
        /// <summary>
        /// Uploads content to Azure Blob Storage with specified settings
        /// </summary>
        /// <param name="content">The products to upload</param>
        /// <param name="options">Upload options including file name, format, etc.</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>True if upload was successful, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when content or options are null</exception>
        /// <exception cref="InvalidOperationException">Thrown when storage container creation or upload fails</exception>
        public async Task<bool> UploadAsync(List<Dictionary<string, object>> content, StorageUploadOptions options, Guid? traceId = null,CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters Action: {action}",
                    traceId,
                    nameof(AzureStorageService),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(UploadAsync),
                    "Starting file upload to Azure Storage",
                    nameof(MethodActionLogTypes.Starting)
                );

                // Validate inputs
                if (content == null) throw new ArgumentNullException(nameof(content));
                if (options == null) throw new ArgumentNullException(nameof(options));

                // Ensure container exists
                var containerExists = await EnsureContainerExistsAsync(traceId, cancellationToken);

                if (!containerExists)
                {
                    throw new InvalidOperationException("Failed to create or verify storage container");
                }

                // Upload file to blob storage
                using var outputStream = new MemoryStream();
                await JsonSerializer.SerializeAsync(outputStream, content, cancellationToken: cancellationToken);
                outputStream.Position = 0;

                // Upload the file
                var success = await storageService.UploadFile(
                    outputStream,
                    options.IsPublic,
                    cancellationToken,
                    options.FileFormat,
                    options.FileNamePostfix
                );

                if (!success)
                {
                    throw new InvalidOperationException("Failed to upload file to storage container");
                }

                logger.LogInformation(
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Message: {message} | Other Parameters Action: {action}",
                    traceId,
                    nameof(AzureStorageService),
                    nameof(LoggingTypes.CheckpointLog),
                    nameof(UploadAsync),
                    "Successfully uploaded file to Azure Storage",
                    nameof(MethodActionLogTypes.Completed)
                );

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "TraceId: {traceId} Service: {serviceName} LogType: {logType} Method: {method} Error Source: {errorSource} Error Message: {errorMessage} Error Stacktrace: {errorStackTrace} Error Inner Exception: {errorInnerException} Internal Message: {internalMessage} | Other Parameters Options: {@options}",
                    traceId,
                    nameof(AzureStorageService),
                    nameof(LoggingTypes.ErrorLog),
                    nameof(UploadAsync),
                    ex.Source,
                    ex.Message,
                    ex.StackTrace,
                    ex.InnerException,
                    "Failed to upload file to Azure Storage",
                    options
                );
                throw;
            }
        }

        /// <summary>
        /// Ensures that the storage container exists, creating it if necessary
        /// </summary>
        /// <param name="storageService">The storage service instance</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>True if the container exists or was successfully created, false otherwise</returns>
        public async Task<bool> EnsureContainerExistsAsync(Guid? traceId = null, CancellationToken cancellationToken = default)
        {
            // Check container exists and create if needed
            var containerExists = await storageService.CreateStorageContainer(cancellationToken);
            if (!containerExists)
            {
                return false;
            }

            return true;
        }
    }
}
