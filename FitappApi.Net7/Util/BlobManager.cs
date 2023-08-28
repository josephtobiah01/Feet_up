using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace FitappApi.Net7.Util
{
    public class BlobManager
    {
        private readonly string _azureBlobAccountUri;
        private readonly string _azureBlobAccountName;
        private readonly string _azureBlobAccountKey;
        private readonly string _blobContainerName;

        public BlobManager(IConfiguration configuration, string containerName)
        {
            _azureBlobAccountUri = configuration["AzureBlobAccountUri"];
            _azureBlobAccountName = configuration["AzureBlobAccountName"];
            _azureBlobAccountKey = configuration["AzureBlobAccountKey"];
            _blobContainerName = containerName;
        }

        public BlobServiceClient GetBlobServiceClient()
        {
            return new BlobServiceClient(new Uri(_azureBlobAccountUri), GetStorageSharedKeyCredential());
        }
        public StorageSharedKeyCredential GetStorageSharedKeyCredential()
        {
            return new StorageSharedKeyCredential(_azureBlobAccountName, _azureBlobAccountKey);
        }
        public string GetSasToken()
        {
            BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _blobContainerName,
                ExpiresOn = DateTime.UtcNow.AddDays(1),//Let SAS token expire after 5 minutes.
            };
            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);//User will only be able to read the blob and it's properties
            return blobSasBuilder.ToSasQueryParameters(GetStorageSharedKeyCredential()).ToString();
        }

        public async Task<string> UploadBlob(string fileContentType, BinaryData fileData)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(_blobContainerName);
            await blobContainerClient.CreateIfNotExistsAsync();


            string blobName = Guid.NewGuid().ToString();
            BlobUploadOptions blobOptions = new BlobUploadOptions();
            if (!string.IsNullOrEmpty(fileContentType))
            {
                string blobContentType = fileContentType;
                //check if file extension was used
                if (!fileContentType.Contains("/"))
                {
                    if (fileContentType.StartsWith("."))
                    {
                        blobName = blobName + fileContentType;
                    }
                    else
                    {
                        blobName = blobName+ "." + fileContentType;
                    }
                    blobContentType = MimeTypeMap.GetMimeType(fileContentType);
                }

                BlobHttpHeaders httpheaders = new BlobHttpHeaders()
                {
                    ContentType = blobContentType
                };
                blobOptions.HttpHeaders = httpheaders;
            }

            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileData, blobOptions);
            return blobClient.Uri.ToString();
        }
    }
}
