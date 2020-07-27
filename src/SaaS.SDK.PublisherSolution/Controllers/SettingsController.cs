namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;

    /// <summary>
    /// Offers Controller.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class SettingsController : BaseController
    {
        private readonly IUsersRepository usersRepository;

        private readonly IValueTypesRepository valueTypesRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IOffersRepository offersRepository;

        private readonly IOfferAttributesRepository offersAttributeRepository;

        private readonly ILogger<OffersController> logger;

        private AppConfigSettingsService appConfigSettingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OffersController"/> class.
        /// </summary>
        /// <param name="offersRepository">The offers repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="valueTypesRepository">The value types repository.</param>
        /// <param name="offersAttributeRepository">The offers attribute repository.</param>
        /// <param name="logger">The logger.</param>
        public SettingsController(IOffersRepository offersRepository, IApplicationConfigRepository applicationConfigRepository, IUsersRepository usersRepository, IValueTypesRepository valueTypesRepository, IOfferAttributesRepository offersAttributeRepository, ILogger<OffersController> logger)
        {
            this.offersRepository = offersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.usersRepository = usersRepository;
            this.valueTypesRepository = valueTypesRepository;
            this.appConfigSettingsService = new AppConfigSettingsService(this.applicationConfigRepository);
            this.offersAttributeRepository = offersAttributeRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All settings.</returns>
        public IActionResult Index()
        {
            this.logger.LogInformation("Settings Controller / Index");
            try
            {
                SettingsModel settings = new SettingsModel();
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);

                settings.appConfigSettings = this.appConfigSettingsService.GetSettings();

                //Making password value to empty for hiding
                foreach (var item in settings.appConfigSettings)
                {
                    if (item.Name.Contains("Password"))
                    {
                        settings.tempsmtppassword = item.Value;
                    }
                }
                return this.View(settings);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <param name="settings">The plans.</param>
        /// <returns>
        /// return All subscription.
        /// </returns>
        [HttpPost]
        public IActionResult SaveSettings(SettingsModel settings)
        {
            this.logger.LogInformation("Settings Controller / SaveSettings:  settings {0}", JsonSerializer.Serialize(settings));
            try
            {
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);
                if (settings != null)
                {
                    foreach (var item in settings.appConfigSettings)
                    {
                        if (!string.IsNullOrEmpty(item.Name))
                        {
                            if (string.IsNullOrEmpty(item.Value) && item.Name.Contains("Password"))
                            {
                                item.Value = settings.tempsmtppassword;
                            }
                            else if (string.IsNullOrEmpty(item.Value))
                            {
                                item.Value = "";
                            }
                            this.appConfigSettingsService.SaveSettings(item);
                        }
                    }
                }
                this.ModelState.Clear();
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.PartialView("Error", ex);
            }
        }
    }
}
