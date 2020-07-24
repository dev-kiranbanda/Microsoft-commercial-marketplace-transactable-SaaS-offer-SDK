namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;

    /// <summary>
    /// App Config Settings Model.
    /// </summary>
    public class AppConfigSettingsModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the App Config.
        /// </summary>
        /// <value>
        /// The name of the Setting.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the App Setting Value.
        /// </summary>
        /// <value>
        /// The value of the setting.
        /// </value>
        public string Value { get; set; }


        /// <summary>
        /// Gets or sets the App Setting Description.
        /// </summary>
        /// <value>
        /// The Description of the setting.
        /// </value>
        public string Description { get; set; }


        /// <summary>
        /// Gets or sets the IsActive of Setting.
        /// </summary>
        /// <value>
        /// Tells Setting is Active or not.
        /// </value>
        public bool IsActive { get; set; }

    }
}
