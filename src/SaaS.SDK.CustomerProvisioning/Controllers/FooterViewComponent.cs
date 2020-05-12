using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.Models;


namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    public class FooterViewComponent : ViewComponent
    {
        /// <summary>
        /// The Configuration
        /// </summary>
        public IConfiguration _iconfiguration { get; }
        private readonly IApplicationConfigRepository applicationConfigRepository;

        public FooterViewComponent(IConfiguration iconfiguration, IApplicationConfigRepository applicationConfigRepository)
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
            Footer model = new Footer();
            //model.Footertext = Convert.ToString(_iconfiguration["FooterText"]);
            model.Footertext = applicationConfigRepository.GetValuefromApplicationConfig("Footer");
            return View("_Footer", model);
        }

    }
}
