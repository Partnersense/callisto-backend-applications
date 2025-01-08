using FeedService.Domain.Internal.MethodOptions;

namespace FeedService.Services.StorageServices
{
    /// <summary>
    /// Generic interface for storage operations that can be implemented by different storage providers
    /// </summary>
    public interface IStorageUploadService
    {
        /// <summary>
        /// Uploads content to storage with specified settings
        /// </summary>
        /// <param name="content">The product to upload</param>
        /// <param name="options">Upload options including file name, format, etc.</param>
        /// <param name="traceId">Unique identifier for request tracing</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>True if upload was successful, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when content or options are null</exception>
        /// <exception cref="InvalidOperationException">Thrown when upload fails</exception>
        Task<bool> UploadAsync(List<Dictionary<string, object>> content, StorageUploadOptions options, Guid? traceId = null, CancellationToken cancellationToken = default);
    }

   
}
