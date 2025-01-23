namespace FeedService.Domain.Internal.MethodOptions
{
    /// <summary>
    /// Provider-agnostic options for storage uploads
    /// </summary>
    public class StorageUploadOptions
    {
        /// <summary>
        /// Name of the file in storage
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Optional format for the file being uploaded (e.g., "json", "xml", etc.)
        /// </summary>
        public string? FileFormat { get; init; }

        /// <summary>
        /// Whether the file should be publicly accessible
        /// </summary>
        public bool IsPublic { get; set; } = false;

        /// <summary>
        /// Optional postfix to append to the file name
        /// </summary>
        public string? FileNamePostfix { get; init; }

        /// <summary>
        /// Additional metadata to store with the file
        /// </summary>
        public IDictionary<string, string>? Metadata { get; set; }
    }
}
