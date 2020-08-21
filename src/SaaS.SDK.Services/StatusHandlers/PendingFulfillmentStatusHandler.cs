namespace Microsoft.Marketplace.SaaS.SDK.Services.StatusHandlers
{
    using System;
    using System.Text.Json;
    using log4net;
    using Microsoft.Extensions.Logging;
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using Microsoft.Marketplace.SaasKit.Contracts;

    /// <summary>
    /// Status handler to handle the subscription in PendingFulfillment.
    /// </summary>
    /// <seealso cref="Microsoft.Marketplace.SaasKit.Provisioning.Webjob.StatusHandlers.AbstractSubscriptionStatusHandler" />
    public class PendingFulfillmentStatusHandler : AbstractSubscriptionStatusHandler
    {
        /// <summary>
        /// The fulfillment API client.
        /// </summary>
        private readonly IFulfillmentApiClient fulfillmentApiClient;

        /// <summary>
        /// The application configuration repository.
        /// </summary>
        private readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// The subscription log repository.
        /// </summary>
        private readonly ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<PendingFulfillmentStatusHandler> _logger;
        protected readonly ILog logger = LogManager.GetLogger(typeof(PendingFulfillmentStatusHandler));

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingFulfillmentStatusHandler" /> class.
        /// </summary>
        /// <param name="fulfillApiClient">The fulfill API client.</param>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        /// <param name="subscriptionsRepository">The subscriptions repository.</param>
        /// <param name="subscriptionLogRepository">The subscription log repository.</param>
        /// <param name="plansRepository">The plans repository.</param>
        /// <param name="usersRepository">The users repository.</param>
        /// <param name="logger">The logger.</param>
        public PendingFulfillmentStatusHandler(
                                                IFulfillmentApiClient fulfillApiClient,
                                                IApplicationConfigRepository applicationConfigRepository,
                                                ISubscriptionsRepository subscriptionsRepository,
                                                ISubscriptionLogRepository subscriptionLogRepository,
                                                IPlansRepository plansRepository,
                                                IUsersRepository usersRepository,
                                                ILogger<PendingFulfillmentStatusHandler> logger)
            : base(subscriptionsRepository, plansRepository, usersRepository)
        {
            this.fulfillmentApiClient = fulfillApiClient;
            this.applicationConfigRepository = applicationConfigRepository;
            this.subscriptionLogRepository = subscriptionLogRepository;
            this._logger = logger;
        }

        /// <summary>
        /// Processes the specified subscription identifier.
        /// </summary>
        /// <param name="subscriptionID">The subscription identifier.</param>
        public override void Process(Guid subscriptionID)
        {
            logger?.InfoFormat("PendingActivationStatusHandler {0}", subscriptionID);
            var subscription = this.GetSubscriptionById(subscriptionID);
            logger?.InfoFormat("Result subscription : {0}", JsonSerializer.Serialize(subscription.AmpplanId));
            logger?.InfoFormat("Get User");
            var userdetails = this.GetUserById(subscription.UserId);

            if (subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.PendingFulfillmentStart.ToString())
            {
                try
                {
                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.PendingActivation.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionStatusEnumExtension.PendingActivation.ToString(),
                        OldValue = SubscriptionStatusEnumExtension.PendingFulfillmentStart.ToString(),
                        CreateBy = userdetails.UserId,
                        CreateDate = DateTime.Now,
                    };
                    this.subscriptionLogRepository.Save(auditLog);
                }
                catch (Exception ex)
                {
                    string errorDescription = string.Format("Exception: {0} :: Innser Exception:{1}", ex.Message, ex.InnerException);

                    logger?.InfoFormat(errorDescription);

                    this.subscriptionsRepository.UpdateStatusForSubscription(subscriptionID, SubscriptionStatusEnumExtension.PendingActivation.ToString(), true);

                    SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                    {
                        Attribute = SubscriptionLogAttributes.Status.ToString(),
                        SubscriptionId = subscription.Id,
                        NewValue = SubscriptionStatusEnumExtension.PendingActivation.ToString(),
                        OldValue = subscription.SubscriptionStatus,
                        CreateBy = userdetails.UserId,
                        CreateDate = DateTime.Now,
                    };
                    this.subscriptionLogRepository.Save(auditLog);
                }
            }
        }
    }
}