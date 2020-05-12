using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.Models;

namespace Microsoft.Marketplace.SaasKit.Client.Controllers
{
    public class LogoViewComponent : ViewComponent
    {
        /// <summary>
        /// The Configuration
        /// </summary>
        public IConfiguration _iconfiguration { get; }
        private readonly IApplicationConfigRepository applicationConfigRepository;

        public LogoViewComponent(IConfiguration iconfiguration, IApplicationConfigRepository applicationConfigRepository)
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
            Logo model = new Logo();
            model.Logolink = applicationConfigRepository.GetValuefromApplicationConfig("ApplicationLogo");
            return View("_Logo", model);
        }

    }
}
