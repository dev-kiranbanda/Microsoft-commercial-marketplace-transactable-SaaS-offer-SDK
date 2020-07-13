namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    public interface IBatchUsageUploadHistoryRepository : IDisposable, IBaseRepository<BatchUsageUploadHistory>
    {
        List<BatchUsageUploadHistory> GetBatchUsageUploadHistoryList(DateTime? UploadedDate, string FileName);
    }
}
