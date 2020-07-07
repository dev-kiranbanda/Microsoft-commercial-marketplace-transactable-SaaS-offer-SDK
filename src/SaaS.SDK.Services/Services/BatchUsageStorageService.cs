namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Marketplace.SaaS.SDK.Services.Contracts;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;


    /// <summary>
    /// Implementation of IARMTemplateStorageService to store the template to Azure blob storage.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaaS.SDK.Services.Contracts.IARMTemplateStorageService" />
    public class BatchUsageStorageService : IBatchUsageStorageService
    {
        /// <summary>
        /// The azure BLOB configuration.
        /// </summary>
        private AzureBlobConfig azureBlobConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorageService"/> class.
        /// </summary>
        /// <param name="azureBlobConfig">The azure BLOB configuration.</param>
        public BatchUsageStorageService(AzureBlobConfig azureBlobConfig)
        {
            this.azureBlobConfig = azureBlobConfig;
        }

        /// <summary>
        /// Saves the batch usage template.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileContantType">Type of the file contant.</param>
        /// <param name="referenceid">The referenceid.</param>
        /// <returns>Reference to the saved Batch Usage (eg:URL). </returns>
        public string UploadFile(IFormFile file, string fileName, string fileContantType, Guid referenceid)
        {
            CloudStorageAccount fileStorageAccount = CloudStorageAccount.Parse(this.azureBlobConfig.BlobConnectionString);

            CloudBlobClient blobClient = fileStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(this.azureBlobConfig.BlobContainer);
            bool result = container.CreateIfNotExistsAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            if (result)
            {
                container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob,
                });
            }

            fileName = fileName.Replace(" ", "-");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = fileContantType;
            blockBlob.UploadFromStreamAsync(file.OpenReadStream(), file.Length);

            return blockBlob.Uri.ToString();
        }

    }
}
