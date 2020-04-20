namespace Microsoft.Marketplace.SaasKit.Models
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Beneficiary Result
    /// </summary>
    public class BeneficiaryResult
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// // Tenant, object id and email address for which SaaS subscription is purchased.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        [JsonProperty("tenantId")]
        public Guid TenantId { get; set; }

        [JsonProperty("emailId")]
        public string EmailId { get; set; }

        [JsonProperty("objectId")]
        public Guid ObjectId { get; set; }
    }
}