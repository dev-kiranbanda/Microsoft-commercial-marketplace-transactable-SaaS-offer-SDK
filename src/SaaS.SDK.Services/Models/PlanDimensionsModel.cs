namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    using System;

    /// <summary>
    /// Plan Events Model.
    /// </summary>
    public class PlanDimensionsModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        public int? PlanId { get; set; }

        /// <summary>
        /// Gets or sets the name of the dimension.
        /// </summary>
        /// <value>
        /// The name of the dimension.
        /// </value>
        public string Dimension { get; set; }

        /// <summary>
        /// Gets or sets the description of the dimension.
        /// </summary>
        /// <value>
        /// The description of the dimension.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the created date of the dimension.
        /// </summary>
        /// <value>
        /// The created date of the dimension.
        /// </value>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the Is Active state of the dimension.
        /// </summary>
        /// <value>
        /// Determines the dimension is active or not.
        /// </value>
        public bool IsActive { get; set; }
    }
}
