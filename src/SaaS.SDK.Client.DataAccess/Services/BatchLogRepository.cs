using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    /// <summary>
    /// Batch Log Repository
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IBatchLogRepository" />
    public class BatchLogRepository : IBatchLogRepository
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
        /// Initializes a new instance of the <see cref="BatchLogRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public BatchLogRepository(SaasKitContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Adds the specified batch logs.
        /// </summary>
        /// <param name="batchLogs">The batch logs.</param>
        /// <returns></returns>
        public int Add(BatchLog batchLogs)
        {
            try
            {
                if (batchLogs.Id == 0)
                {
                    Context.BatchLog.Add(batchLogs);
                    Context.SaveChanges();
                    return batchLogs.Id;
                }
                else
                {
                    Context.BatchLog.Update(batchLogs);
                    Context.SaveChanges();
                    return batchLogs.Id;
                }
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
        public IEnumerable<BatchLog> Get()
        {
            return Context.BatchLog;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public BatchLog Get(int id)
        {
            return Context.BatchLog.Where(s => s.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Remove(BatchLog entity)
        {
            Context.BatchLog.Remove(entity);
            Context.SaveChanges();
        }


        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns> entity id.</returns>
        public int Save(BatchLog entity)
        {
            if (entity.Id == 0)
            {
                Context.BatchLog.Add(entity);
                Context.SaveChanges();
                return entity.Id;
            }
            else
            {
                Context.BatchLog.Update(entity);
                Context.SaveChanges();
                return entity.Id;
            }
        }
    }
}