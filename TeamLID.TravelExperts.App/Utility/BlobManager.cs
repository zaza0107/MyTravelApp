using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamLID.TravelExperts.App.Utility
{
    public class BlobManager
    {
        public static IConfiguration Configuration { get; set; }
        public static CloudBlobContainer Container { get; set; }
        public static CloudBlobClient BlobClient { get; set; }
        public static CloudStorageAccount StorageAccount { get; set; }
        public BlobManager(IConfiguration configuration)
        {
            //init service and container client
            //https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet#code-examples
            //https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/storage/Azure.Storage.Blobs/samples/Sample01a_HelloWorld.cs
            Configuration = configuration;
        }
        public async Task CreateCloudBlobContainerAsync()
        {
            if (Container == null)
            {
                StorageAccount = CloudStorageAccount.Parse(
                    Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONNECTION_STRING") ??
                    Configuration.GetConnectionString("AZURE_BLOB_STORAGE_CONNECTION_STRING")
                );
                BlobClient = StorageAccount.CreateCloudBlobClient();
                Container = BlobClient.GetContainerReference(
                    Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONTAINER_REFERENCE") ??
                    Configuration.GetConnectionString("AZURE_BLOB_STORAGE_CONTAINER_REFERENCE")
                );
                await Container.CreateIfNotExistsAsync();
            }
        }
        public async Task<string> UploadFileToStorageAsync(Stream fileStream, string fileName)
        {
            try
            {
                if (Container == null)
                {
                    await CreateCloudBlobContainerAsync();
                }
                CloudBlockBlob blockBlob = Container.GetBlockBlobReference(fileName);
                await blockBlob.UploadFromStreamAsync(fileStream);
                return blockBlob.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public async Task<bool> DeleteFileFromStorageAsync(string fileName)
        {
            try
            {
                if (Container == null)
                {
                    await CreateCloudBlobContainerAsync();
                }
                CloudBlockBlob blockBlob = Container.GetBlockBlobReference(fileName);
                await blockBlob.DeleteIfExistsAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
