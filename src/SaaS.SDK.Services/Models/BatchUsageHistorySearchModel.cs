using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    public class BatchUsageHistorySearchModel
    {
        public DateTime? UploadDate { get; set; }

        public string Filename { get; set; }

        public List<BatchUsageUploadHistory> batchUsageUploadHistorylist { get; set; }

    }
}
