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
    public class FooterViewComponent : ViewComponent
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

        public FooterViewComponent(IConfiguration iconfiguration, IApplicationConfigRepository applicationConfigRepository,
            ILogger<LogoViewComponent> logger)
        {
            _iconfiguration = iconfiguration;
            this.applicationConfigRepository = applicationConfigRepository;
            this._logger = logger;
        }
        /// <summary>
        /// Load footer using the ViewComponent
        /// </summary>
        /// <returns></returns>
        /// 
        public IViewComponentResult Invoke()
        {
            Footer model = new Footer();
            try
            {
                //model.Footertext = Convert.ToString(_iconfiguration["FooterText"]);
                model.Footertext = applicationConfigRepository.GetValueByName("Footer");
                return View("_Footer", model);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                model.Footertext = "Unable to load footer";
                return View("_Footer", model);
            }
        }

    }
}
