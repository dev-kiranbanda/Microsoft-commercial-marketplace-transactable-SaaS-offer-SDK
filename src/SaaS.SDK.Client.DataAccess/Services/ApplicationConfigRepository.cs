﻿namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Repository to access ApplicationConfiguration.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts.IApplicationConfigRepository" />
    public class ApplicationConfigRepository : IApplicationConfigRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly SaasKitContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfigRepository"/> class.
        /// </summary>
        /// <param name="context">The this.context.</param>
        public ApplicationConfigRepository(SaasKitContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the name of the value by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Value of application configuration entry by name.
        /// </returns>
        public string GetValueByName(string name)
        {
            return this.context.ApplicationConfiguration.Where(s => s.Name == name).FirstOrDefault().Value;
        }

        /// <summary>
        /// Gets the value from application configuration.
        /// </summary>
        /// <returns>List of all application configuration items.</returns>
        public IEnumerable<ApplicationConfiguration> GetAll()
        {
            return this.context.ApplicationConfiguration.Where(s => s.IsActive == true);
        }

        /// <summary>
        /// Saves Settings.
        /// </summary>
        /// <param name="appConfigSetting">The plan attributes.</param>
        /// <returns> App Config Id.</returns>
        public int? SavePlanSetting(ApplicationConfiguration appConfigSetting)
        {
            if (appConfigSetting != null)
            {
                var existingSetting = this.context.ApplicationConfiguration.Where(s => s.Name ==
                appConfigSetting.Name).FirstOrDefault();
                if (existingSetting != null)
                {
                    existingSetting.Id = appConfigSetting.Id;
                    existingSetting.Name = appConfigSetting.Name;
                    existingSetting.Value = appConfigSetting.Value;
                    existingSetting.Description = appConfigSetting.Description;
                    existingSetting.IsActive = appConfigSetting.IsActive;

                    this.context.ApplicationConfiguration.Update(existingSetting);
                    this.context.SaveChanges();
                    return existingSetting.Id;
                }
                else
                {
                    appConfigSetting.IsActive = true;
                    appConfigSetting.Description = appConfigSetting.Name;
                    this.context.ApplicationConfiguration.Add(appConfigSetting);
                    this.context.SaveChanges();
                    return appConfigSetting.Id;
                }
            }
            return null;
        }
    }
}
