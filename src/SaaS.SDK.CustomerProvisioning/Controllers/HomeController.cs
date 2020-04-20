﻿namespace Microsoft.Marketplace.SaasKit.Client.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.CustomerProvisioning.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Client.Helpers;
    using Microsoft.Marketplace.SaasKit.Client.Models;
    using Microsoft.Marketplace.SaasKit.Client.Services;
    using Microsoft.Marketplace.SaasKit.Contracts;
    using Microsoft.Marketplace.SaasKit.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using SaasKitModels = Microsoft.Marketplace.SaasKit.Models;

    /// <summary>Home Controller</summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Web.Controllers.BaseController"/>
    public class HomeController : BaseController
    {
        /// <summary>
        /// Defines the  API Client
        /// </summary>
        private readonly IFulfillmentApiClient apiClient;

        /// <summary>
        /// The subscription repository
        /// </summary>
        private readonly ISubscriptionsRepository subscriptionRepository;

        /// <summary>
        /// The subscription logs repository
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The application log repository
        /// </summary>
        private readonly IApplicationLogRepository applicationLogRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        private readonly IPlansRepository planRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        private readonly IOffersRepository offersRepository;

        /// <summary>
        /// The user repository
        /// </summary>
        private readonly IUsersRepository userRepository;

        private readonly ILogger<HomeController> logger;
        /// <summary>
        /// The subscription service
        /// </summary>
        private SubscriptionService subscriptionService = null;

        /// <summary>
        /// The application log service
        /// </summary>
        private ApplicationLogService applicationLogService = null;

        /// <summary>
        /// The user service
        /// </summary>
        private UserService userService;

        private readonly IApplicationConfigRepository applicationConfigRepository;

        private readonly IEmailTemplateRepository emailTemplateRepository;

        private readonly IPlanEventsMappingRepository planEventsMappingRepository;



        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="apiClient">The API Client<see cref="IFulfilmentApiClient" /></param>
        /// <param name="subscriptionRepo">The subscription repository.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="applicationLogRepository">The application log repository.</param>
        /// <param name="subscriptionLogsRepo">The subscription logs repository.</param>
        public HomeController(ILogger<HomeController> logger, IFulfillmentApiClient apiClient, ISubscriptionsRepository subscriptionRepo, IPlansRepository planRepository, IUsersRepository userRepository, IApplicationLogRepository applicationLogRepository, ISubscriptionLogRepository subscriptionLogsRepo, IApplicationConfigRepository applicationConfigRepository, IEmailTemplateRepository emailTemplateRepository, IOffersRepository offersRepository, IPlanEventsMappingRepository planEventsMappingRepository)
        {
            this.apiClient = apiClient;
            this.subscriptionRepository = subscriptionRepo;
            this.subscriptionLogRepository = subscriptionLogsRepo;
            this.applicationLogRepository = applicationLogRepository;
            this.planRepository = planRepository;
            this.userRepository = userRepository;
            this.userService = new UserService(this.userRepository);
            this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository);
            this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
            this.applicationConfigRepository = applicationConfigRepository;
            this.emailTemplateRepository = emailTemplateRepository;
            this.planEventsMappingRepository = planEventsMappingRepository;

            this.logger = logger;
            this.offersRepository = offersRepository;
        }

        #region View Action Methods

        /// <summary>
        /// Get All Subscription List for Current Logged in User
        /// </summary>
        /// <param name="token">The MS Token<see cref="string" /></param>
        /// <returns>
        /// The <see cref="IActionResult" />
        /// </returns>
        public IActionResult Index(string token = null)
        {
            try
            {
                this.logger.LogInformation($"Landing page with token {token}");
                //SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();
                //this.log.Info("Initializing Index Page");
                SubscriptionResult subscriptionDetail = new SaasKitModels.SubscriptionResult();

                SubscriptionResultExtension subscriptionExtension = new SubscriptionResultExtension();

                if (User.Identity.IsAuthenticated)
                {
                    if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                    {
                        this.TempData["ShowLicensesMenu"] = true;
                    }
                    var userId = this.userService.AddPartnerDetail(GetCurrentUserDetail());
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);

                    this.logger.LogInformation("User authenticated successfully");

                    if (!string.IsNullOrEmpty(token))
                    {
                        this.TempData["ShowWelcomeScreen"] = null;
                        token = token.Replace(' ', '+');
                        var newSubscription = this.apiClient.ResolveAsync(token).ConfigureAwait(false).GetAwaiter().GetResult();

                        if (newSubscription != null && newSubscription.SubscriptionId != default)
                        {
                            var subscriptionPlanDetail = this.apiClient.GetAllPlansForSubscriptionAsync(newSubscription.SubscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();

                            Offers offers = new Offers()
                            {
                                OfferId = newSubscription.OfferId,
                                OfferName = newSubscription.OfferId,
                                UserId = currentUserId,
                                CreateDate = DateTime.Now,
                                OfferGuid = Guid.NewGuid()
                            };

                            Guid newOfferId = this.offersRepository.Add(offers);

                            List<PlanDetailResultExtension> planList = new List<PlanDetailResultExtension>();
                            var serializedPlans = JsonConvert.SerializeObject(subscriptionPlanDetail);
                            planList = JsonConvert.DeserializeObject<List<PlanDetailResultExtension>>(serializedPlans);
                            planList.ForEach(x =>
                            {
                                x.OfferId = newOfferId;
                                x.PlanGUID = Guid.NewGuid();
                            });

                            this.subscriptionService.AddPlanDetailsForSubscription(planList);
                            var currentPlan = this.planRepository.GetPlanDetailByPlanId(newSubscription.PlanId);
                            // GetSubscriptionBy SubscriptionId
                            var subscriptionData = this.apiClient.GetSubscriptionByIdAsync(newSubscription.SubscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();


                            var subscribeId = this.subscriptionService.AddUpdatePartnerSubscriptions(subscriptionData);

                            if (subscribeId > 0 && subscriptionData.SaasSubscriptionStatus == SubscriptionStatusEnum.PendingFulfillmentStart)
                            {
                                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                                {
                                    Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                                    SubscriptionId = subscribeId,
                                    NewValue = SubscriptionStatusEnum.PendingFulfillmentStart.ToString(),
                                    OldValue = "None",
                                    CreateBy = currentUserId,
                                    CreateDate = DateTime.Now
                                };
                                this.subscriptionLogRepository.Add(auditLog);
                            }


                            subscriptionDetail = subscriptionData;
                            subscriptionExtension.ShowWelcomeScreen = false;
                            subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                            subscriptionDetail.CustomerName = this.CurrentUserName;


                            var serializedSubscription = JsonConvert.SerializeObject(subscriptionDetail);
                            subscriptionExtension = JsonConvert.DeserializeObject<SubscriptionResultExtension>(serializedSubscription);
                            subscriptionExtension.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(newSubscription.SubscriptionId, currentPlan.PlanGuid);
                            //return this.PartialView("SubscriptionsHome", subscriptionExtension);
                        }
                    }
                    else
                    {
                        this.TempData["ShowWelcomeScreen"] = "True";
                        subscriptionExtension.ShowWelcomeScreen = true;
                        return this.View(subscriptionExtension);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        return this.Challenge(new AuthenticationProperties
                        {
                            RedirectUri = "/?token=" + token
                        }, OpenIdConnectDefaults.AuthenticationScheme);
                    }
                    else
                    {
                        this.TempData["ShowWelcomeScreen"] = "True";
                        subscriptionExtension.ShowWelcomeScreen = true;
                        return this.View(subscriptionExtension);
                    }
                }



                return this.View(subscriptionExtension);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Subscription this instance.
        /// </summary>
        /// <returns> Subscription instance</returns>
        public IActionResult Subscriptions()
        {
            logger.LogInformation("Home Controller / Subscriptions ");
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                    {
                        this.TempData["ShowLicensesMenu"] = true;
                    }
                    this.TempData["ShowWelcomeScreen"] = "True";
                    SubscriptionViewModel subscriptionDetail = new SubscriptionViewModel();
                    subscriptionDetail.Subscriptions = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, default, true).ToList();
                    foreach (var subscription in subscriptionDetail.Subscriptions)
                    {
                        Plans PlanDetail = this.planRepository.GetPlanDetailByPlanId(subscription.PlanId);
                        subscriptionDetail.IsAutomaticProvisioningSupported = Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig("IsAutomaticProvisioningSupported"));
                        subscription.IsPerUserPlan = PlanDetail.IsPerUser.HasValue ? PlanDetail.IsPerUser.Value : false;
                    }
                    subscriptionDetail.SaaSAppUrl = this.apiClient.GetSaaSAppURL();

                    if (this.TempData["ErrorMsg"] != null)
                    {
                        subscriptionDetail.IsSuccess = false;
                        subscriptionDetail.ErrorMessage = Convert.ToString(this.TempData["ErrorMsg"]);
                    }

                    return this.View(subscriptionDetail);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Get All Subscription List for Current Logged in User
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>
        /// The <see cref="IActionResult" />
        /// </returns>
        public IActionResult SubscriptionDetail(Guid subscriptionId)
        {
            this.logger.LogInformation("Home Controller / SubscriptionDetail subscriptionId:{0}", JsonConvert.SerializeObject(subscriptionId));
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                    {
                        this.TempData["ShowLicensesMenu"] = true;
                    }
                    var subscriptionDetail = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                    subscriptionDetail.PlanList = this.subscriptionService.GetAllSubscriptionPlans();

                    return this.View(subscriptionDetail);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Get Subscription Details for selected Subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns>
        /// The <see cref="IActionResult" />
        /// </returns>
        public IActionResult SubscriptionQuantityDetail(Guid subscriptionId)
        {
            this.logger.LogInformation("Home Controller / SubscriptionQuantityDetail subscriptionId:{0}", JsonConvert.SerializeObject(subscriptionId));
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var subscriptionDetail = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                    return this.View(subscriptionDetail);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Subscriptions the log detail.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <returns> Subscription log detail</returns>
        public IActionResult SubscriptionLogDetail(Guid subscriptionId)
        {
            this.logger.LogInformation("Home Controller / SubscriptionQuantityDetail subscriptionId:{0}", JsonConvert.SerializeObject(subscriptionId));
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    List<SubscriptionAuditLogs> subscriptionAudit = new List<SubscriptionAuditLogs>();
                    subscriptionAudit = this.subscriptionLogRepository.GetSubscriptionBySubscriptionId(subscriptionId).ToList();
                    return this.View(subscriptionAudit);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// The Error
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult" />
        /// </returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionDetail = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            return this.View(exceptionDetail?.Error);
        }
        #endregion

        #region Operation Methods

        /// <summary>
        /// Subscriptions the operation.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>Subscriptions operation</returns>
        [HttpPost]
        public IActionResult SubscriptionOperation(SubscriptionResultExtension subscriptionResultExtension, Guid subscriptionId, string planId, string operation)
        {
            this.logger.LogInformation("Home Controller / SubscriptionOperation subscriptionId:{0} :: planId : {1} :: operation:{2}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(planId), JsonConvert.SerializeObject(operation));
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }

                bool isSuccess = false;
                if (subscriptionId != default)
                {
                    SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();
                    SubscriptionResultExtension oldsubscriptionDetail = new SubscriptionResultExtension();
                    this.logger.LogInformation("GetPartnerSubscription");
                    var oldValue = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                    this.logger.LogInformation("GetUserIdFromEmailAddress");
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    string newStatus = string.Empty;

                    if (operation == "Activate")
                    {
                        try
                        {
                            oldsubscriptionDetail = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                            Plans PlanDetail = this.planRepository.GetPlanDetailByPlanId(oldsubscriptionDetail.PlanId);
                            oldsubscriptionDetail.GuidPlanId = PlanDetail.PlanGuid;
                          
                            this.logger.LogInformation("operation == Activate");
                            if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig("IsAutomaticProvisioningSupported")))
                            {
                                this.logger.LogInformation("UpdateStateOfSubscription PendingActivation: SubscriptionId: {0} ", subscriptionId);
                                newStatus = "PendingActivation";
                                this.subscriptionService.UpdateStateOfSubscription(subscriptionId, SubscriptionStatusEnum.PendingActivation, true);
                                subscriptionDetail.SaasSubscriptionStatus = SubscriptionStatusEnum.PendingActivation;
                            }
                            else
                            {
                                this.logger.LogInformation("ActivateSubscriptionAsync Subscribed: SubscriptionId: {0} ", subscriptionId);
                                newStatus = "Subscribed";
                                var response = this.apiClient.ActivateSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false).GetAwaiter().GetResult();
                                this.logger.LogInformation("UpdateStateOfSubscription Subscribed: SubscriptionId: {0} ", subscriptionId);
                                this.subscriptionService.UpdateStateOfSubscription(subscriptionId, SubscriptionStatusEnum.Subscribed, true);
                            }
                            subscriptionDetail = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                            PlanDetail = this.planRepository.GetPlanDetailByPlanId(subscriptionDetail.PlanId);
                            subscriptionDetail.GuidPlanId = PlanDetail.PlanGuid;
                            isSuccess = true;
                            // do not remove or duplicate below assignments.
                            subscriptionDetail.SubscriptionParameters = subscriptionResultExtension.SubscriptionParameters;
                            subscriptionDetail.Purchaser = subscriptionResultExtension.Purchaser;
                            subscriptionDetail.Beneficiary = subscriptionResultExtension.Beneficiary;
                            subscriptionDetail.OfferId = subscriptionResultExtension.OfferId;

                            this.logger.LogInformation("GetPartnerSubscription and GetAllSubscriptionPlans");
                            subscriptionDetail.PlanList = this.subscriptionService.GetAllSubscriptionPlans();
                            this.logger.LogInformation("Save Subscription Parameters:  {0}", JsonConvert.SerializeObject(subscriptionResultExtension.SubscriptionParameters));
                            if (subscriptionResultExtension.SubscriptionParameters != null && subscriptionResultExtension.SubscriptionParameters.Count() > 0)
                            {
                                this.subscriptionService.AddSubscriptionParameters(subscriptionResultExtension.SubscriptionParameters, currentUserId);
                            }
                            //  var subscriptionData = this.apiClient.GetSubscriptionByIdAsync(subscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                            //var serializedParent = JsonConvert.SerializeObject(subscriptionDetail);
                            //subscriptionDetail = JsonConvert.DeserializeObject<SubscriptionResult>(serializedParent);

                            this.logger.LogInformation("checkIsActive");
                            bool checkIsActive = emailTemplateRepository.GetIsActive(subscriptionDetail.SaasSubscriptionStatus.ToString()).HasValue ? emailTemplateRepository.GetIsActive(subscriptionDetail.SaasSubscriptionStatus.ToString()).Value : false;
                            if (subscriptionDetail.SaasSubscriptionStatus == SubscriptionStatusEnum.Subscribed && Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(EmailTriggerConfigurationConstants.ISEMAILENABLEDFORSUBSCRIPTIONACTIVATION)) == true)
                            {
                                this.logger.LogInformation("SendEmail to {0} :: Template{1} ", JsonConvert.SerializeObject(applicationConfigRepository), JsonConvert.SerializeObject(emailTemplateRepository));
                                EmailHelper.SendEmail(subscriptionDetail, applicationConfigRepository, emailTemplateRepository, planEventsMappingRepository);
                            }
                            else if (subscriptionDetail.SaasSubscriptionStatus == SubscriptionStatusEnum.PendingActivation && Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(EmailTriggerConfigurationConstants.ISEMAILENABLEDFORPENDINGACTIVATION)) == true)
                            {
                                this.logger.LogInformation("SendEmail to {0} :: Template{1} ", JsonConvert.SerializeObject(applicationConfigRepository), JsonConvert.SerializeObject(emailTemplateRepository));
                                EmailHelper.SendEmail(subscriptionDetail, applicationConfigRepository, emailTemplateRepository, planEventsMappingRepository);
                            }
                        }
                        catch (FulfillmentException fex)
                        {
                            this.TempData["ErrorMsg"] = fex.Message;
                            EmailHelper.SendEmail(oldsubscriptionDetail, applicationConfigRepository, emailTemplateRepository, planEventsMappingRepository, "failure", oldValue.SaasSubscriptionStatus, newStatus);
                        }
                    }

                    if (operation == "Deactivate")
                    {
                        try
                        {
                            oldsubscriptionDetail = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId, true).FirstOrDefault();
                            Plans PlanDetail = this.planRepository.GetPlanDetailByPlanId(oldsubscriptionDetail.PlanId);
                            oldsubscriptionDetail.GuidPlanId = PlanDetail.PlanGuid;
                            this.logger.LogInformation("operation == Deactivate");
                            this.logger.LogInformation("DeleteSubscriptionAsync");
                            newStatus = "Unsubscribed";
                            var response = this.apiClient.DeleteSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false).GetAwaiter().GetResult();
                            this.logger.LogInformation("UpdateStateOfSubscription");
                            this.subscriptionService.UpdateStateOfSubscription(subscriptionId, SubscriptionStatusEnum.Unsubscribed, false);
                            isSuccess = true;
                            this.logger.LogInformation("GetPartnerSubscription");
                            this.logger.LogInformation("GetAllSubscriptionPlans");
                            subscriptionDetail.SaasSubscriptionStatus = SubscriptionStatusEnum.Unsubscribed;
                            subscriptionDetail.PlanList = this.subscriptionService.GetAllSubscriptionPlans();


                            //  var subscriptionData = this.apiClient.GetSubscriptionByIdAsync(subscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                            //var serializedParent = JsonConvert.SerializeObject(subscriptionDetail);
                            //subscriptionDetail = JsonConvert.DeserializeObject<SubscriptionResult>(serializedParent);
                            bool checkIsActive = emailTemplateRepository.GetIsActive(subscriptionDetail.SaasSubscriptionStatus.ToString()).HasValue ? emailTemplateRepository.GetIsActive(subscriptionDetail.SaasSubscriptionStatus.ToString()).Value : false;
                            if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(EmailTriggerConfigurationConstants.ISEMAILENABLEDFORUNSUBSCRIPTION)) == true)
                            {
                                this.logger.LogInformation("SendEmail to {0} :: Template{1} ", JsonConvert.SerializeObject(applicationConfigRepository), JsonConvert.SerializeObject(emailTemplateRepository));

                                EmailHelper.SendEmail(subscriptionDetail, applicationConfigRepository, emailTemplateRepository, planEventsMappingRepository);
                            }
                        }
                        catch (FulfillmentException fex)
                        {
                            this.TempData["ErrorMsg"] = fex.Message;
                            EmailHelper.SendEmail(oldsubscriptionDetail, applicationConfigRepository, emailTemplateRepository, planEventsMappingRepository, "failure", oldValue.SaasSubscriptionStatus, newStatus);
                        }
                    }
                    this.logger.LogInformation("GetPartnerSubscription");
                    var newValue = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId, true).FirstOrDefault();
                    if (isSuccess)
                    {
                        if (oldValue != null && newValue != null)
                        {
                            SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                            {
                                Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                                SubscriptionId = newValue.SubscribeId,
                                NewValue = Convert.ToString(newValue.SaasSubscriptionStatus),
                                OldValue = Convert.ToString(oldValue.SaasSubscriptionStatus),
                                CreateBy = currentUserId,
                                CreateDate = DateTime.Now
                            };
                            this.subscriptionLogRepository.Add(auditLog);
                        }
                    }
                }

                return this.RedirectToAction(nameof(this.ActivatedMessage));
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        public IActionResult ActivatedMessage()
        {
            try
            {
                return this.PartialView();
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("Home Controller / ActivatedMessage Exception: {0}", ex);
                return View("Error", ex);
            }
        }

        public IActionResult ActivateSubscription(Guid subscriptionId, string planId, string operation)
        {
            this.logger.LogInformation("Home Controller / ActivateSubscription subscriptionId:{0} :: planId : {1} :: operation:{2}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(planId), JsonConvert.SerializeObject(operation));
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

                if (User.Identity.IsAuthenticated)
                {
                    var userId = this.userService.AddPartnerDetail(GetCurrentUserDetail());
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);


                    this.TempData["ShowWelcomeScreen"] = false;
                    var subscriptionData = this.apiClient.GetSubscriptionByIdAsync(subscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                    //var subscribeId = this.subscriptionService.AddUpdatePartnerSubscriptions(subscriptionData);
                    var oldValue = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();

                    var serializedParent = JsonConvert.SerializeObject(subscriptionData);
                    subscriptionDetail = JsonConvert.DeserializeObject<SubscriptionResultExtension>(serializedParent);
                    var planDetails = this.planRepository.GetPlanDetailByPlanId(subscriptionData.PlanId);
                    subscriptionDetail.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, planDetails.PlanGuid);
                    //subscriptionDetail = (SubscriptionResult)subscriptionData;
                    subscriptionDetail.ShowWelcomeScreen = false;
                    subscriptionDetail.SaasSubscriptionStatus = SubscriptionStatusEnum.PendingFulfillmentStart;
                    subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                    subscriptionDetail.CustomerName = this.CurrentUserName;
                }
                return this.View("Index", subscriptionDetail);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        public IActionResult DeActivateSubscription(Guid subscriptionId, string planId, string operation)
        {
            try
            {
                if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

                if (User.Identity.IsAuthenticated)
                {
                    var userId = this.userService.AddPartnerDetail(GetCurrentUserDetail());
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);


                    this.TempData["ShowWelcomeScreen"] = false;
                    var subscriptionData = this.apiClient.GetSubscriptionByIdAsync(subscriptionId).ConfigureAwait(false).GetAwaiter().GetResult();
                    var subscribeId = this.subscriptionService.AddUpdatePartnerSubscriptions(subscriptionData);
                    var oldValue = this.subscriptionService.GetPartnerSubscription(CurrentUserEmailAddress, subscriptionId).FirstOrDefault();

                    //var serializedParent = JsonConvert.SerializeObject(subscriptionData);
                    //subscriptionDetail = JsonConvert.DeserializeObject<SubscriptionResult>(serializedParent);
                    //subscriptionDetail = (SubscriptionResult)subscriptionData;

                    var serializedParent = JsonConvert.SerializeObject(subscriptionData);
                    subscriptionDetail = JsonConvert.DeserializeObject<SubscriptionResultExtension>(serializedParent);

                    subscriptionDetail.ShowWelcomeScreen = false;
                    subscriptionDetail.SaasSubscriptionStatus = SubscriptionStatusEnum.Subscribed;
                    subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                    subscriptionDetail.CustomerName = this.CurrentUserName;
                }
                return this.View("Index", subscriptionDetail);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Changes the subscription plan.
        /// </summary>
        /// <param name="subscriptionDetail">The subscription detail.</param>
        /// <returns>Changes subscription plan</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeSubscriptionPlan(SubscriptionResult subscriptionDetail)
        {
            this.logger.LogInformation("Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{0}", JsonConvert.SerializeObject(subscriptionDetail));
            try
            {
                var subscriptionId = new Guid();
                var planId = string.Empty;
                if (Convert.ToBoolean(applicationConfigRepository.GetValuefromApplicationConfig(MainMenuStatusEnum.IsLicenseManagementEnabled.ToString())) == true)
                {
                    this.TempData["ShowLicensesMenu"] = true;
                }
                if (subscriptionDetail != null)
                {
                    subscriptionId = subscriptionDetail.Id;
                    planId = subscriptionDetail.PlanId;
                }

                if (subscriptionId != default && !string.IsNullOrEmpty(planId))
                {
                    try
                    {
                        var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);

                        var jsonResult = await this.apiClient.ChangePlanForSubscriptionAsync(subscriptionId, planId).ConfigureAwait(false);

                        var changePlanOperationStatus = OperationStatusEnum.InProgress;
                        if (jsonResult != null && jsonResult.OperationId != default)
                        {
                            while (OperationStatusEnum.InProgress.Equals(changePlanOperationStatus) || OperationStatusEnum.NotStarted.Equals(changePlanOperationStatus))
                            {
                                var changePlanOperationResult = await this.apiClient.GetOperationStatusResultAsync(subscriptionId, jsonResult.OperationId).ConfigureAwait(false);
                                changePlanOperationStatus = changePlanOperationResult.Status;

                                this.logger.LogInformation("Operation Status :  " + changePlanOperationStatus + " For SubscriptionId " + subscriptionId + "Model SubscriptionID): {0} :: planID:{1}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(planId));
                                this.applicationLogService.AddApplicationLog("Operation Status :  " + changePlanOperationStatus + " For SubscriptionId " + subscriptionId);
                            }

                            var oldValue = this.subscriptionService.GetSubscriptionsForSubscriptionId(subscriptionId);

                            this.subscriptionService.UpdateSubscriptionPlan(subscriptionId, planId);
                            this.logger.LogInformation("Plan Successfully Changed.");
                            this.applicationLogService.AddApplicationLog("Plan Successfully Changed.");

                            if (oldValue != null)
                            {
                                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                                {
                                    Attribute = Convert.ToString(SubscriptionLogAttributes.Plan),
                                    SubscriptionId = oldValue.SubscribeId,
                                    NewValue = planId,
                                    OldValue = oldValue.PlanId,
                                    CreateBy = currentUserId,
                                    CreateDate = DateTime.Now
                                };
                                this.subscriptionLogRepository.Add(auditLog);
                            }
                        }
                    }
                    catch (FulfillmentException fex)
                    {
                        this.TempData["ErrorMsg"] = fex.Message;
                    }
                }

                return this.RedirectToAction(nameof(this.Subscriptions));
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }

        /// <summary>
        /// Changes the quantity plan.
        /// </summary>
        /// <param name="subscriptionDetail">The subscription detail.</param>
        /// <returns>Changes subscription quantity</returns>
        [HttpPost]
        public async Task<IActionResult> ChangeSubscriptionQuantity(SubscriptionResult subscriptionDetail)
        {
            this.logger.LogInformation("Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{0}", JsonConvert.SerializeObject(subscriptionDetail));
            try
            {
                if (subscriptionDetail != null && subscriptionDetail.Id != default && subscriptionDetail.Quantity > 0)
                {
                    try
                    {
                        var subscriptionId = subscriptionDetail.Id;
                        var quantity = subscriptionDetail.Quantity;

                        var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);

                        var jsonResult = await this.apiClient.ChangeQuantityForSubscriptionAsync(subscriptionId, quantity).ConfigureAwait(false);

                        var changeQuantityOperationStatus = OperationStatusEnum.InProgress;
                        if (jsonResult != null && jsonResult.OperationId != default)
                        {
                            while (OperationStatusEnum.InProgress.Equals(changeQuantityOperationStatus) || OperationStatusEnum.NotStarted.Equals(changeQuantityOperationStatus))
                            {
                                var changeQuantityOperationResult = await this.apiClient.GetOperationStatusResultAsync(subscriptionId, jsonResult.OperationId).ConfigureAwait(false);
                                changeQuantityOperationStatus = changeQuantityOperationResult.Status;

                                this.logger.LogInformation("changeQuantity Operation Status :  " + changeQuantityOperationStatus + " For SubscriptionId " + subscriptionId + "Model SubscriptionID): {0} :: quantity:{1}", JsonConvert.SerializeObject(subscriptionId), JsonConvert.SerializeObject(quantity));
                                this.applicationLogService.AddApplicationLog("Operation Status :  " + changeQuantityOperationStatus + " For SubscriptionId " + subscriptionId);
                            }

                            var oldValue = this.subscriptionService.GetSubscriptionsForSubscriptionId(subscriptionId);

                            this.subscriptionService.UpdateSubscriptionQuantity(subscriptionId, quantity);
                            this.logger.LogInformation("Quantity Successfully Changed.");
                            this.applicationLogService.AddApplicationLog("Quantity Successfully Changed.");

                            if (oldValue != null)
                            {
                                SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                                {
                                    Attribute = Convert.ToString(SubscriptionLogAttributes.Quantity),
                                    SubscriptionId = oldValue.SubscribeId,
                                    NewValue = quantity.ToString(),
                                    OldValue = oldValue.Quantity.ToString(),
                                    CreateBy = currentUserId,
                                    CreateDate = DateTime.Now
                                };
                                this.subscriptionLogRepository.Add(auditLog);
                            }
                        }
                    }
                    catch (FulfillmentException fex)
                    {
                        this.TempData["ErrorMsg"] = fex.Message;
                        this.logger.LogError("Message:{0} :: {1}   ", fex.Message, fex.InnerException);
                    }
                }

                return this.RedirectToAction(nameof(this.Subscriptions));
            }
            catch (Exception ex)
            {
                this.logger.LogError("Message:{0} :: {1}   ", ex.Message, ex.InnerException);
                return View("Error", ex);
            }
        }
        #endregion
    }
}
