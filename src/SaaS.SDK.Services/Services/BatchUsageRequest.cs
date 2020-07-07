namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;

    /// <summary>
    /// Batch Usage Request
    /// </summary>
    public class BatchUsageRequest
    {
        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        public string SubscriptionID { get; set; }

        /// <summary>
        /// Gets or sets the type of the API.
        /// </summary>
        /// <value>
        /// The type of the API.
        /// </value>
        public string APIType { get; set; }

        /// <summary>
        /// Gets or sets the consumed units.
        /// </summary>
        /// <value>
        /// The consumed units.
        /// </value>
        public string ConsumedUnits { get; set; }
    }
}
