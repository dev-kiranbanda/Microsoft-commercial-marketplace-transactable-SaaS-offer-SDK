﻿namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access ARM Templates.
    /// </summary>
    public interface IArmTemplateRepository
    {
        /// <summary>
        /// Save the specified template details.
        /// </summary>
        /// <param name="templateDetails">The template details.</param>
        /// <returns>ID of the ARM Template that is saved.</returns>
        Guid? Save(Armtemplates templateDetails);

        /// <summary>
        /// Saves the template parameters.
        /// </summary>
        /// <param name="templateParameters">The template parameters.</param>
        /// <returns> Arm Template Id.</returns>
        Guid? SaveParameters(ArmtemplateParameters templateParameters);

        /// <summary>
        /// Gets all ARM Templates.
        /// </summary>
        /// <returns>List of ARM Templates.</returns>
        IEnumerable<Armtemplates> GetAll();

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="armTemplateId">The arm template identifier.</param>
        /// <returns> Armtemplates.</returns>
        Armtemplates GetById(Guid? armTemplateId);
    }
}
