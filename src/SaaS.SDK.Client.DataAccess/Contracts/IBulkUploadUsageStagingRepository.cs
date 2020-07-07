using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    /// <summary>
    /// Bulk UploadUsage Staging Repository Interface
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBaseRepository{Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities.BulkUploadUsageStaging}" />
    public interface IBulkUploadUsageStagingRepository : IDisposable, IBaseRepository<BulkUploadUsageStaging>
    {
        /// <summary>
        /// Validates the bulk upload usage staging.
        /// </summary>
        /// <param name="batchLogId">The batch log identifier.</param>
        /// <returns></returns>
        List<BulkUploadUsageStagingResult> ValidateBulkUploadUsageStaging(int batchLogId);

        /// <summary>
        /// Gets the bulk upload usage meters.
        /// </summary>
        /// <param name="batchLogId">The batch log identifier.</param>
        /// <returns></returns>
        List<UploadUsageMetersResult> GetBulkUploadUsageMeters(int batchLogId);
    }
}
