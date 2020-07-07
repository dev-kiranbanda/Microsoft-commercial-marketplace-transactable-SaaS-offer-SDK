namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.DataModel
{
    /// <summary>
    /// Upload Usage Meters Result
    /// </summary>
    public class UploadUsageMetersResult
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the billable units.
        /// </summary>
        /// <value>
        /// The billable units.
        /// </value>
        public int BillableUnits { get; set; }

        /// <summary>
        /// Gets or sets the remaining units.
        /// </summary>
        /// <value>
        /// The remaining units.
        /// </value>
        public int RemainingUnits { get; set; }

        /// <summary>
        /// Gets or sets the meters by API type identifier.
        /// </summary>
        /// <value>
        /// The meters by API type identifier.
        /// </value>
        public int MetersByApiTypeID { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        public string Sequence { get; set; }

        /// <summary>
        /// Gets or sets the type of the API.
        /// </summary>
        /// <value>
        /// The type of the API.
        /// </value>
        public string ApiType { get; set; }

        /// <summary>
        /// Gets or sets the meter.
        /// </summary>
        /// <value>
        /// The meter.
        /// </value>
        public string Meter { get; set; }

        /// <summary>
        /// Gets or sets the minimum qty.
        /// </summary>
        /// <value>
        /// The minimum qty.
        /// </value>
        public decimal MinQty { get; set; }

        /// <summary>
        /// Gets or sets the maximum qty.
        /// </summary>
        /// <value>
        /// The maximum qty.
        /// </value>
        public decimal MaxQty { get; set; }

        /// <summary>
        /// Gets or sets the batch log identifier.
        /// </summary>
        /// <value>
        /// The batch log identifier.
        /// </value>
        public int BatchLogID { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        public string SubscriptionID { get; set; }

        /// <summary>
        /// Gets or sets the consumed units.
        /// </summary>
        /// <value>
        /// The consumed units.
        /// </value>
        public decimal ConsumedUnits { get; set; }
    }
}
