using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using StaffApp.Application.Services;

namespace StaffApp.Infrastructure.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly string _connectionString;

        public AzureBlobService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"];
        }

        private async Task<BlobContainerClient> GetContainerClientAsync(string containerName)
        {
            var containerClient = new BlobContainerClient(_connectionString, containerName);

            // Create the container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync();

            return containerClient;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string containerName)
        {
            var containerClient = await GetContainerClientAsync(containerName);

            // Get a reference to a blob
            var blobClient = containerClient.GetBlobClient(fileName);

            // Set the content type
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            // Upload the file
            await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

            // Return the URI to the blob (this is a private URI)
            return blobClient.Uri.ToString();
        }

        public async Task<BlobDownloadInfo> DownloadFileAsync(string blobName, string containerName)
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            // Check if the blob exists
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"The file {blobName} does not exist in container {containerName}.");
            }

            // Download the blob
            var response = await blobClient.DownloadAsync();
            return response.Value;
        }

        public async Task DeleteFileAsync(string blobName, string containerName)
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            // Delete the blob
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> GenerateSasTokenForBlobAsync(string blobName, string containerName, DateTimeOffset expiresOn)
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            // Check if the blob exists
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"The file {blobName} does not exist in container {containerName}.");
            }

            // Create a SAS token that's valid for the specified duration
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b", // b for blob
                ExpiresOn = expiresOn
            };

            // Set permissions for the SAS token
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Generate the SAS token
            var blobUriBuilder = new BlobUriBuilder(blobClient.Uri)
            {
                Sas = sasBuilder.ToSasQueryParameters(new Azure.Storage.StorageSharedKeyCredential(
                    accountName: GetAccountName(_connectionString),
                    accountKey: GetAccountKey(_connectionString)))
            };

            // Return the full URI with SAS token
            return blobUriBuilder.ToString();
        }

        // Helper methods to extract account name and key from connection string
        private string GetAccountName(string connectionString)
        {
            var parts = connectionString.Split(';');
            foreach (var part in parts)
            {
                if (part.StartsWith("AccountName="))
                {
                    return part.Substring("AccountName=".Length);
                }
            }
            throw new ArgumentException("AccountName not found in connection string");
        }

        private string GetAccountKey(string connectionString)
        {
            var parts = connectionString.Split(';');
            foreach (var part in parts)
            {
                if (part.StartsWith("AccountKey="))
                {
                    return part.Substring("AccountKey=".Length);
                }
            }
            throw new ArgumentException("AccountKey not found in connection string");
        }
    }
}
