using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Azure;
using System.IO;
using Microsoft.Extensions.Logging;
using DocumentFormat.OpenXml.Spreadsheet;
using Azure.Storage;

namespace FileUploadApp.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileService> _logger;
        string _storageconnection = string.Empty;
        private string _containerName = "files";
        public FileService(IConfiguration configuration, ILogger<FileService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _storageconnection = _configuration.GetConnectionString("AzureStorageAccount");
        }
        public async Task<string> UploadBlobAsync(string fileName, string contentType, Stream fileStream, string email)
        {
            try
            {
                var containerClient = new BlobContainerClient(_storageconnection, _containerName);
                var createResponse = await containerClient.CreateIfNotExistsAsync();
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                {
                    await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
                }
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
                try
                {
                    IDictionary<string, string> metadata = new Dictionary<string, string>();
                    metadata["email"] = email;
                    await blobClient.SetMetadataAsync(metadata);
                }
                catch (RequestFailedException ex)
                {
                    _logger?.LogError(ex.ToString());
                    throw;
                }
                var urlString = blobClient.Uri.ToString();
                return urlString;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<string> GetSASToken(string fileName)
        {
            try
            {
                var azureStorageAccount = _configuration.GetSection("AzureStorage:AzureAccount").Value;
                var azureStorageAccessKey = _configuration.GetSection("AzureStorage:AccessKey").Value;
                Azure.Storage.Sas.BlobSasBuilder blobSasBuilder = new Azure.Storage.Sas.BlobSasBuilder()
                {
                    BlobContainerName = _containerName,
                    BlobName = fileName,
                    ExpiresOn = DateTime.UtcNow.AddHours(1),
                };
                blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);
                var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(azureStorageAccount,
                    azureStorageAccessKey)).ToString();
                return sasToken;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
