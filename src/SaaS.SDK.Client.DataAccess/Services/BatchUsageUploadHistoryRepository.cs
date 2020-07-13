namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBatchUsageUploadHistoryRepository" />
    public class BatchUsageUploadHistoryRepository : IBatchUsageUploadHistoryRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        public SaasKitContext Context = new SaasKitContext();

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchUsageUploadHistoryRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public BatchUsageUploadHistoryRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Adds the specified batchusagehistory.
        /// </summary>
        /// <param name="batchusagehistory">The batchusagehistory.</param>
        /// <returns></returns>
        public int Save(BatchUsageUploadHistory batchusagehistory)
        {
            try
            {
                Context.BatchUsageUploadHistory.Add(batchusagehistory);
                Context.SaveChanges();
                return batchusagehistory.Id;
            }
            catch (Exception) { }
            return 0;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BatchUsageUploadHistory> Get()
        {
            return Context.BatchUsageUploadHistory;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public BatchUsageUploadHistory Get(int id)
        {
            return Context.BatchUsageUploadHistory.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Remove(BatchUsageUploadHistory entity)
        {
            Context.BatchUsageUploadHistory.Remove(entity);
            Context.SaveChanges();
        }

        public List<BatchUsageUploadHistory> GetBatchUsageUploadHistoryList(DateTime? UploadedDate, string FileName)
        {
            return Context.BatchUsageUploadHistory.FromSqlRaw("dbo.spGetBatchUsageHistory {0},{1}", UploadedDate, FileName).ToList();
        }
    }
}
