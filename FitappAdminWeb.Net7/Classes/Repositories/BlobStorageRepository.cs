using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace FitappAdminWeb.Net7.Classes.Repositories
{
    public class BlobStorageRepository
    {
        BlobServiceClient _blobClient;
        ILogger<BlobStorageRepository> _logger;

        public static readonly string FILEUPLOAD_ERRORSTRING = "fileupload_error";

        public BlobStorageRepository(BlobServiceClient blobClient, ILogger<BlobStorageRepository> logger)
        {
            _blobClient = blobClient;
            _logger = logger;
        }

        public async Task<string> UploadImageUrl(byte[] imageBytes, string contentType)
        {
            try
            {
                var blobContainerClient = _blobClient.GetBlobContainerClient("images");
                await blobContainerClient.CreateIfNotExistsAsync();
                BlobHttpHeaders httpheaders = new BlobHttpHeaders()
                {
                    ContentType = contentType
                };

                string filename = Guid.NewGuid().ToString() + GetFileExtensionFromContentType(contentType);
                BinaryData data = new BinaryData(imageBytes);

                var bc = blobContainerClient.GetBlobClient(filename);
                await bc.UploadAsync(data, new BlobUploadOptions() {  HttpHeaders = httpheaders });

                //return blob url
                return bc.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload image to blob");
                return FILEUPLOAD_ERRORSTRING;
            }
        }

        private string GetFileExtensionFromContentType(string contentType)
        {
            contentType = contentType.ToLower();
            switch (contentType)
            {
                case "image/png":
                    return ".png";
                case "image/jpeg":
                    return ".jpeg";
                case "image/jpg":
                    return ".jpg";
                default:
                    return String.Empty;
            }
        }
    }
}
