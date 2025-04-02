using Azure.Storage.Blobs.Models;

namespace StaffApp.Application.Services
{
    public interface IAzureBlobService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string containerName);
        Task<BlobDownloadInfo> DownloadFileAsync(string blobName, string containerName);
        Task DeleteFileAsync(string blobName, string containerName);
        Task<string> GenerateSasTokenForBlobAsync(string blobName, string containerName, DateTimeOffset expiresOn);
    }
}
