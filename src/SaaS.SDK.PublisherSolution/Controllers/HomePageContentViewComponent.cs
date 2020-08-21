using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    public class HomePageContentViewComponent : ViewComponent
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<HomePageContentViewComponent> _logger;
        protected readonly ILog logger = LogManager.GetLogger(typeof(HomePageContentViewComponent));

        /// <summary>
        /// The Configuration
        /// </summary>
        public IConfiguration _iconfiguration { get; }
        private readonly IApplicationConfigRepository applicationConfigRepository;

        public HomePageContentViewComponent(IConfiguration iconfiguration, IApplicationConfigRepository applicationConfigRepository, ILogger<HomePageContentViewComponent> logger)
        {
            this._logger = logger;
            _iconfiguration = iconfiguration;
            this.applicationConfigRepository = applicationConfigRepository;
        }
        /// <summary>
        /// Load footer using the ViewComponent
        /// </summary>
        /// <returns></returns>
        /// 
        public IViewComponentResult Invoke()
        {
            HomePageContent model = new HomePageContent();
            try
            {
                model.HomeContent = applicationConfigRepository.GetValueByName("PublisherHomePageContent");
                return View("_HomePageContent", model);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                model.HomeContent = "Unable to load Home page content";
                return this.View("_HomePageContent", model);
            }
        }

    }
}
