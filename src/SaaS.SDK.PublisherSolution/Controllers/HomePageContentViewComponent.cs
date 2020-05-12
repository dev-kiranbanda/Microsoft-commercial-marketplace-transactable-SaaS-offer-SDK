using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Marketplace.Saas.Web.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using Microsoft.Marketplace.SaasKit.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    public class HomePageContentViewComponent : ViewComponent
    {
        /// <summary>
        /// The Configuration
        /// </summary>
        public IConfiguration _iconfiguration { get; }
        private readonly IApplicationConfigRepository applicationConfigRepository;

        public HomePageContentViewComponent(IConfiguration iconfiguration, IApplicationConfigRepository applicationConfigRepository)
        {
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
            model.HomeContent = applicationConfigRepository.GetValuefromApplicationConfig("HomePageContent");
            return View("_HomePageContent", model);
        }

    }
}
