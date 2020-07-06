namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Service to enable operations over settings.
    /// </summary>
    public class AppConfigSettingsService
    {
        /// <summary>
        /// The app config repository repository.
        /// </summary>
        private IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigSettingsService"/> class.
        /// </summary>
        /// <param name="applicationConfigRepository">The offer repo.</param>
        public AppConfigSettingsService(IApplicationConfigRepository applicationConfigRepository)
        {
            this.applicationConfigRepository = applicationConfigRepository;
        }

        /// <summary>
        /// Gets the App Config Settings.
        /// </summary>
        /// <returns> App Config Model.</returns>
        public List<AppConfigSettingsModel> GetSettings()
        {
            List<AppConfigSettingsModel> settingslist = new List<AppConfigSettingsModel>();
            var allsettingData = this.applicationConfigRepository.GetAll();
            foreach (var item in allsettingData)
            {
                AppConfigSettingsModel setting = new AppConfigSettingsModel();
                setting.Id = item.Id;
                setting.Name = item.Name;
                setting.Value = item.Value;
                setting.Description = item.Description;
                settingslist.Add(setting);
            }
            return settingslist;
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <param name="appConfigSetting">The settings.</param>
        /// <returns> App Setting Id.</returns>
        public int? SaveSettings(AppConfigSettingsModel appConfigSetting)
        {
            if (appConfigSetting != null)
            {
                ApplicationConfiguration setting = new ApplicationConfiguration();
                setting.Id = appConfigSetting.Id;
                setting.Name = appConfigSetting.Name;
                setting.Value = appConfigSetting.Value;
                setting.Description = appConfigSetting.Description;

                var appConfigSettingId = this.applicationConfigRepository.SavePlanSetting(setting);
                return appConfigSettingId;
            }

            return null;
        }
    }
}