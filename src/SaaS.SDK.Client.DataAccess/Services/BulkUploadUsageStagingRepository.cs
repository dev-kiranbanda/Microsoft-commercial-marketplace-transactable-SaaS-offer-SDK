namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;


    /// <summary>
    /// Bulk Upload Usage Staging Repository
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBulkUploadUsageStagingRepository" />
    public class BulkUploadUsageStagingRepository : IBulkUploadUsageStagingRepository
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
        /// Initializes a new instance of the <see cref="BulkUploadUsageStagingRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public BulkUploadUsageStagingRepository(SaasKitContext context)
        {
            Context = context;
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
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
        public IEnumerable<BulkUploadUsageStaging> Get()
        {
            return Context.BulkUploadUsageStaging;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public BulkUploadUsageStaging Get(int id)
        {
            return Context.BulkUploadUsageStaging.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Adds the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public int Save(BulkUploadUsageStaging entities)
        {
            try
            {
                Context.BulkUploadUsageStaging.Add(entities);
                Context.SaveChanges();
                return entities.Id;
            }
            catch (Exception) { }
            return 0;
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Remove(BulkUploadUsageStaging entity)
        {
            Context.BulkUploadUsageStaging.Remove(entity);
            Context.SaveChanges();
        }

        /// <summary>
        /// Validates the bulk upload usage staging.
        /// </summary>
        /// <param name="batchLogId">The batch log identifier.</param>
        /// <returns></returns>
        public List<BulkUploadUsageStagingResult> ValidateBulkUploadUsageStaging(int batchLogId)
        {
            try
            {
                return Context.BulkUploadUsageStagingResult.FromSqlRaw("dbo.spValidateBulkUploadUsage {0}", batchLogId).ToList();
            }
            catch (Exception)
            { }
            return new List<BulkUploadUsageStagingResult>();
        }

        /// <summary>
        /// Gets the bulk upload usage meters.
        /// </summary>
        /// <param name="batchLogId">The batch log identifier.</param>
        /// <returns></returns>
        public List<UploadUsageMetersResult> GetBulkUploadUsageMeters(int batchLogId)
        {
            try
            {
                return Context.UploadUsageMetersResult.FromSqlRaw("dbo.spGetBulkUploadUsageMeters {0}", batchLogId).ToList<UploadUsageMetersResult>();
            }
            catch (Exception)
            { }
            return new List<UploadUsageMetersResult>();
        }
    }
}
