namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Credentials Model.
    /// </summary>
    public class CredentialsModel
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        [JsonPropertyName("Tenant ID")]
        public string TenantID { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>
        /// The subscription identifier.
        /// </value>
        [JsonPropertyName("Subscription ID")]
        public string SubscriptionID { get; set; }

        /// <summary>
        /// Gets or sets the service principal identifier.
        /// </summary>
        /// <value>
        /// The service principal identifier.
        /// </value>
        [JsonPropertyName("Service Principal ID")]
        public string ServicePrincipalID { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        [JsonPropertyName("Client Secret")]
        public string ClientSecret { get; set; }
    }
}
