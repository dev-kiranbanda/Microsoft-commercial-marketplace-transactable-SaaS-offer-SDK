namespace Microsoft.Marketplace.SaaS.SDK.Services.Contracts
{
    using System;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Contract to manage Batch Usage.
    /// </summary>
    public interface IBatchUsageStorageService
    {
        /// <summary>
        /// Saves the batch usage template.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileContantType">Type of the file contant.</param>
        /// <param name="referenceid">The referenceid.</param>
        /// <returns>Reference to the saved Batch Usage (eg:URL). </returns>
        string UploadFile(IFormFile file, string fileName, string fileContantType, Guid referenceid);

    }
}
