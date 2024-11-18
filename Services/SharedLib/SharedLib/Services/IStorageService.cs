namespace SharedLib.Services;

public interface IStorageService
{
    Task<bool> CreateStorageContainer(CancellationToken cancellationToken);
    Task<bool> UploadFile(Stream file, bool isPublic, CancellationToken cancellationToken, string? fileFormat = "", string? blobNamePostfix = "");
}
