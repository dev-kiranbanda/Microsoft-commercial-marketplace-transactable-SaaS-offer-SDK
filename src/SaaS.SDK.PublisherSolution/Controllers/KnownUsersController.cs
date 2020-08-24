namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using log4net;
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
    public class KnownUsersController : BaseController
    {

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<KnownUsersController> _logger;
        protected readonly ILog logger = LogManager.GetLogger(typeof(KnownUsersController));

        private readonly IKnownUsersRepository KownUsersRepository;

        private readonly IUsersRepository usersRepository;

        private KnownUserService knownUserService;

        public KnownUsersController(IKnownUsersRepository knownUsersRepository, ILogger<KnownUsersController> logger, IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
            knownUserService = new KnownUserService(knownUsersRepository);
            this._logger = logger;
        }


        public IActionResult Index()
        {
            logger.InfoFormat("Offers Controller / Index");
            try
            {
                KnownUsersModel knownUsers = new KnownUsersModel();
                this.TempData["ShowWelcomeScreen"] = "True";
                var currentUserDetail = this.usersRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);

                var knownUsersList = this.knownUserService.GetAllKnownUsers();
                knownUsers.KnownUsersList = knownUsersList;
                return this.View(knownUsers);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return this.View("Error", new Exception("An error occured while fetching offers."));
            }
        }

        [HttpPost]
        public IActionResult AddKnownUser(KnownUsersModel knownuser)
        {
            try
            {
                this.knownUserService.AddKnownUser(knownuser);

                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                logger.InfoFormat("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error");
            }
        }

        public IActionResult RemoveKnownUser(int knownUsersId, string knownUserEmail)
        {
            try
            {
                KnownUsersModel knownusers = new KnownUsersModel()
                {
                    KnownUsersId = knownUsersId,
                    KnownUsers = knownUserEmail,
                };

                this.knownUserService.RemoveKnownUser(knownusers);

                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                logger.InfoFormat("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error");
            }
        }

    }
}
