﻿namespace Microsoft.Marketplace.Saas.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaaS.SDK.Services.Services;
    using Microsoft.Marketplace.SaaS.SDK.Services.Utilities;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;

    /// <summary>
    /// Plans Controller.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.Saas.Web.Controllers.BaseController" />
    [ServiceFilter(typeof(KnownUserAttribute))]
    public class BatchUsageHistoryController : BaseController
    {
        /// <summary>
        /// The subscription repository.
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The users repository.
        /// </summary>
        private readonly IUsersRepository usersRepository;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IPlansRepository plansRepository;

        private readonly IOffersRepository offerRepository;

        private readonly IOfferAttributesRepository offerAttributeRepository;

        private readonly ILogger<OffersController> logger;

        private readonly IBatchUsageUploadHistoryRepository batchUsageUploadHistoryRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="PlansController" /> class.
        /// </summary>
        /// <param name="subscriptionRepository">The subscription repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="plansRepository">The plans repository.</param>
        /// <param name="offerAttributeRepository">The offer attribute repository.</param>
        /// <param name="offerRepository">The offer repository.</param>
        /// <param name="logger">The logger.</param>
        public BatchUsageHistoryController(ISubscriptionsRepository subscriptionRepository, IUsersRepository usersRepository, IApplicationConfigRepository applicationConfigRepository, IPlansRepository plansRepository, IOfferAttributesRepository offerAttributeRepository, IOffersRepository offerRepository, ILogger<OffersController> logger, IBatchUsageUploadHistoryRepository batchUsageUploadHistoryRepository)
        {
            this.subscriptionRepository = subscriptionRepository;
            this.usersRepository = usersRepository;
            this.applicationConfigRepository = applicationConfigRepository;
            this.plansRepository = plansRepository;
            this.offerAttributeRepository = offerAttributeRepository;
            this.offerRepository = offerRepository;
            this.logger = logger;
            this.batchUsageUploadHistoryRepository = batchUsageUploadHistoryRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>return All subscription.</returns>
        public IActionResult BatchUsageHistory()
        {
            this.logger.LogInformation("BatchUsageHistoryController / OfferDetails:  offerGuId");
            try
            {
                BatchUsageHistorySearchModel batchUsageHistorySearch = new BatchUsageHistorySearchModel();
                batchUsageHistorySearch.Filename = null;
                batchUsageHistorySearch.UploadDate = null;
                batchUsageHistorySearch.batchUsageUploadHistorylist = batchUsageUploadHistoryRepository.GetBatchUsageUploadHistoryList(batchUsageHistorySearch.UploadDate, batchUsageHistorySearch.Filename);
                return View(batchUsageHistorySearch);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                return this.View("Error", ex);
            }
        }

        /// <summary>
        /// Batches the usage history.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Search(DateTime? uploadDate, string fileName)
        {
            logger.LogInformation("Home Controller / BatchUsageHistory ");
            try
            {
                BatchUsageHistorySearchModel searchCriteria = new BatchUsageHistorySearchModel();
                searchCriteria.UploadDate = uploadDate;
                if (!string.IsNullOrEmpty(fileName)){
                    searchCriteria.Filename = fileName.Trim();
                }
                searchCriteria.batchUsageUploadHistorylist = batchUsageUploadHistoryRepository.GetBatchUsageUploadHistoryList(searchCriteria.UploadDate, searchCriteria.Filename);
                Thread.Sleep(1000);
                return this.PartialView("BatchUsageHistoryList", searchCriteria);
            }
            catch (Exception ex)
            {
                logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return this.View("Error", ex);
            }
        }
    }
}