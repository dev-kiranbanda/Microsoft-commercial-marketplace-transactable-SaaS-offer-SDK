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


namespace Microsoft.Marketplace.SaasKit.Client.Controllers
{
    public class LogoViewComponent : ViewComponent
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<LogoViewComponent> _logger;
        protected readonly ILog logger = LogManager.GetLogger(typeof(LogoViewComponent));

        /// <summary>
        /// The Configuration
        /// </summary>
        public IConfiguration _iconfiguration { get; }
        private readonly IApplicationConfigRepository applicationConfigRepository;

        public LogoViewComponent(IConfiguration iconfiguration, IApplicationConfigRepository applicationConfigRepository, ILogger<LogoViewComponent> logger)
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
            Logo model = new Logo();
            try
            {
                model.Logolink = applicationConfigRepository.GetValueByName("ApplicationLogo");
                return View("_Logo", model);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                model.Logolink = "Unable to load Logo";
                return View("_Logo", model);

            }
        }

    }
}
