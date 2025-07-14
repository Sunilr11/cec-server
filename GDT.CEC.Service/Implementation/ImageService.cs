using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GDT.CEC.Service.Implementation
{
    public class ImageService : IImageService
    {
        private readonly IOptions<AzureStorageConfig> _storageConfig;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IOptions<AzureStorageConfig> storageConfig, ILogger<ImageService> logger)
        {
            _storageConfig = storageConfig;
            _logger = logger;
        }



        public async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            string imgurl = "";
            try
            {
                string fileExt = System.IO.Path.GetExtension(imageFile.FileName);
                var uri = new Uri(_storageConfig.Value.ContainerUrl + "?" + _storageConfig.Value.Signature);
                var segments = uri.AbsolutePath.Split('/');
                var containerName = segments.Length > 1 ? segments[1] : string.Empty;
                var blobServiceClient = new BlobServiceClient(uri);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                string newFileName = Guid.NewGuid().ToString() + fileExt;
                var blobClient = blobContainerClient.GetBlobClient(newFileName);
                using (var stream = imageFile.OpenReadStream())
                {
                    var info = await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = imageFile.ContentType });
                }
                imgurl = _storageConfig.Value.ContainerUrl + "/" + newFileName + "?" + _storageConfig.Value.Signature;
                return imgurl;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading image: {ex.Message}", ex);
                return imgurl;
            }
        }

        public async Task<string> UploadImageToBlobAsync(byte[] bytes, string filename, string contentType)
        {
            string imgurl = "";
            try
            {
                string fileExt = System.IO.Path.GetExtension(filename);
                var uri = new Uri(_storageConfig.Value.ContainerUrl + "?" + _storageConfig.Value.Signature);
                var segments = uri.AbsolutePath.Split('/');
                var containerName = segments.Length > 1 ? segments[1] : string.Empty;
                var blobServiceClient = new BlobServiceClient(uri);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                string newFileName = Guid.NewGuid().ToString() + fileExt;
                var blobClient = blobContainerClient.GetBlobClient(newFileName);

                using (var stream = new MemoryStream(bytes))
                {
                    var info = await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });
                }
                imgurl = _storageConfig.Value.ContainerUrl + "/" + newFileName + "?" + _storageConfig.Value.Signature;
                return imgurl;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading image: {ex.Message}", ex);
                throw;
            }
        }
    }
}
