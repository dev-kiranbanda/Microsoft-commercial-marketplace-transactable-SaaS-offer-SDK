using CsvHelper.Configuration;
using Microsoft.Marketplace.SaasKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using  Microsoft.Marketplace.SaaS.SDK.Services.Services;


namespace Microsoft.Marketplace.Saas.Web.Mapper
{
    public sealed class BatchUsageMapper : ClassMap<BatchUsageRequest>
    {
        public BatchUsageMapper()
        {
            Map(m => m.SubscriptionID).Name("SubscriptionID");
            Map(m => m.APIType).Name("APIType");
            Map(m => m.ConsumedUnits).Name("ConsumedUnits");
        }
    }
}
