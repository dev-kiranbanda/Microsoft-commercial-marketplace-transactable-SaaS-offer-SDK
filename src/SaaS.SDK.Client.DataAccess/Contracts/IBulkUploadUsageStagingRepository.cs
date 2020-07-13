using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    
    public interface IBulkUploadUsageStagingRepository : IDisposable, IBaseRepository<BulkUploadUsageStaging>
    {
        /// <summary>
        /// Gets the bulk upload staging records.
        /// </summary>
        /// <param name="batchLogId">The batch log identifier.</param>
        /// <returns></returns>
        List<BulkUploadUsageStaging> GetByBatchLogId(int batchLogId);
    }
}
