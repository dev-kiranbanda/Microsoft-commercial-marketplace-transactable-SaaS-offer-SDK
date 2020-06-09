namespace Microsoft.Marketplace.SaasKit.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    public class MeteringBatchUsageResult : SaaSApiResult
    {
        [JsonProperty("result")]
        public List<MeteringUsageResult> BatchUsageResponse { get; set; }
    }
}