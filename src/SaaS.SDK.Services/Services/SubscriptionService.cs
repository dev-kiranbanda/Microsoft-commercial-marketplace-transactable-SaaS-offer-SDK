﻿using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using SaasKitModels = Microsoft.Marketplace.SaasKit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Marketplace.SaasKit.Models;
//using Microsoft.Marketplace.SaasKit.Client.Models;
using Microsoft.Marketplace.SaaS.SDK.Services.Models;
using Newtonsoft.Json;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    public class SubscriptionService
    {
        /// <summary>
        /// The subscription repository
        /// </summary>
        public ISubscriptionsRepository SubscriptionRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        public IPlansRepository PlanRepository;

        /// <summary>
        /// The plan repository
        /// </summary>
        public ISubscriptionLogRepository subscriptionLogRepository;

        /// <summary>
        /// The current user identifier
        /// </summary>
        public int CurrentUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionService" /> class.
        /// </summary>
        /// <param name="subscriptionRepo">The subscription repository.</param>
        /// <param name="planRepository">The plan repository.</param>
        /// <param name="currentUserId">The current user identifier.</param>
        public SubscriptionService(ISubscriptionsRepository subscriptionRepo, IPlansRepository planRepository, int currentUserId = 0)
        {
            SubscriptionRepository = subscriptionRepo;
            PlanRepository = planRepository;
            CurrentUserId = currentUserId;
        }

        /// <summary>
        /// Adds/Update partner subscriptions.
        /// </summary>
        /// <param name="subscriptionDetail">The subscription detail.</param>
        public int AddUpdatePartnerSubscriptions(SaasKitModels.SubscriptionResult subscriptionDetail)
        {
            var isActive = GetSubscriptionStateFromStatus(Convert.ToString(subscriptionDetail.SaasSubscriptionStatus));
            Subscriptions newSubscription = new Subscriptions()
            {
                Id = 0,
                AmpplanId = subscriptionDetail.PlanId,
                Ampquantity = subscriptionDetail.Quantity,
                AmpsubscriptionId = subscriptionDetail.Id,
                CreateBy = CurrentUserId,
                CreateDate = DateTime.Now,
                IsActive = isActive,
                ModifyDate = DateTime.Now,
                Name = subscriptionDetail.Name,
                SubscriptionStatus = Convert.ToString(subscriptionDetail.SaasSubscriptionStatus),
                UserId = CurrentUserId,
            };
            return SubscriptionRepository.Add(newSubscription);
        }

        /// <summary>
        /// Binds the subscriptions.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="status">The status.</param>
        /// <param name="isActivate">if set to <c>true</c> [is activate].</param>
        public void UpdateStateOfSubscription(Guid subscriptionId, string status, bool isActivate)
        {
            SubscriptionRepository.UpdateStatusForSubscription(subscriptionId, status, isActivate);
        }

        ///// <summary>
        ///// Gets the subscriptions for subscription identifier.
        ///// </summary>
        ///// <param name="subscriptionId">The subscription identifier.</param>
        ///// <param name="includeUnsubscribed">if set to <c>true</c> [include unsubscribed].</param>
        ///// <returns></returns>
        //public SubscriptionResult GetSubscriptionsForSubscriptionId(Guid subscriptionId, bool includeUnsubscribed = false)
        //{
        //    var subscriptionDetail = SubscriptionRepository.GetSubscriptionsByScheduleId(subscriptionId);
        //    if (subscriptionDetail != null)
        //    {
        //        SubscriptionResult subscritpionDetail = PrepareSubscriptionResponse(subscriptionDetail);
        //        if (subscritpionDetail != null)
        //            return subscritpionDetail;
        //    }
        //    return new SubscriptionResult();
        //}

        /// <summary>
        /// Subscriptions state from status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public bool GetSubscriptionStateFromStatus(string status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                if (Convert.ToString(SubscriptionStatusEnum.Unsubscribed) == status)
                    return false;
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the subscriptions for partner.
        /// </summary>
        /// <param name="partnerEmailAddress">The partner email address.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="includeUnsubscribed">if set to <c>true</c> [include unsubscribed].</param>
        /// <returns></returns>
        public List<SubscriptionResultExtension> GetPartnerSubscription(string partnerEmailAddress, Guid subscriptionId, bool includeUnsubscribed = false)
        {
            List<SubscriptionResultExtension> allSubscriptions = new List<SubscriptionResultExtension>();
            var allSubscriptionsForEmail = SubscriptionRepository.GetSubscriptionsByEmailAddress(partnerEmailAddress, subscriptionId, includeUnsubscribed).OrderByDescending(s => s.CreateDate).ToList();

            foreach (var subscription in allSubscriptionsForEmail)
            {
                SubscriptionResultExtension subscritpionDetail = PrepareSubscriptionResponse(subscription);
                if (subscritpionDetail != null && subscritpionDetail.SubscribeId > 0)
                    allSubscriptions.Add(subscritpionDetail);
            }
            return allSubscriptions;
        }

        /// <summary>
        /// Gets the subscriptions for subscription identifier.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="includeUnsubscribed">if set to <c>true</c> [include unsubscribed].</param>
        /// <returns></returns>
        public SubscriptionResultExtension GetSubscriptionsBySubscriptionId(Guid subscriptionId, bool includeUnsubscribed = false)
        {
            var subscriptionDetail = SubscriptionRepository.GetSubscriptionsByScheduleId(subscriptionId, includeUnsubscribed);
            if (subscriptionDetail != null)
            {
                SubscriptionResultExtension subscritpionDetail = PrepareSubscriptionResponse(subscriptionDetail);
                if (subscritpionDetail != null)
                    return subscritpionDetail;
            }
            return new SubscriptionResultExtension();
        }

        /// <summary>
        /// Prepares the subscription response.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <returns></returns>
        public SubscriptionResultExtension PrepareSubscriptionResponse(Subscriptions subscription)
        {
            var existingPlanDetail = this.PlanRepository.GetPlanDetailByPlanId(subscription.AmpplanId);

            SubscriptionResultExtension subscritpionDetail = new SubscriptionResultExtension
            {
                Id = subscription.AmpsubscriptionId,
                SubscribeId = subscription.Id,
                PlanId = string.IsNullOrEmpty(subscription.AmpplanId) ? string.Empty : subscription.AmpplanId,
                Quantity = subscription.Ampquantity,
                Name = subscription.Name,
                SubscriptionStatus = GetSubscriptionStatus(subscription.SubscriptionStatus),
                IsActiveSubscription = subscription.IsActive ?? false,
                CustomerEmailAddress = subscription.User?.EmailAddress,
                CustomerName = subscription.User?.FullName,
                IsMeteringSupported = existingPlanDetail != null ? (existingPlanDetail.IsmeteringSupported ?? false) : false

            };
            return subscritpionDetail;
        }

        /// <summary>
        /// Gets the subscriptions for partner.
        /// </summary>
        /// <param name="partnerEmailAddress">The partner email address.</param>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="includeUnsubscribed">if set to <c>true</c> [include unsubscribed].</param>
        /// <returns></returns>
        public List<SubscriptionResult> GetPartnerSubscriptions(string partnerEmailAddress, Guid subscriptionId, bool includeUnsubscribed = false)
        {
            List<SubscriptionResult> allSubscriptions = new List<SubscriptionResult>();
            var allSubscriptionsForEmail = SubscriptionRepository.GetSubscriptionsByEmailAddress(partnerEmailAddress, subscriptionId, includeUnsubscribed).OrderByDescending(s => s.CreateDate).ToList();

            foreach (var subscription in allSubscriptionsForEmail)
            {
                SubscriptionResult subscritpionDetail = PrepareSubscriptionResponse(subscription);
                if (subscritpionDetail != null && subscritpionDetail.SubscribeId > 0)
                    allSubscriptions.Add(subscritpionDetail);
            }

            return allSubscriptions;
        }


        public SubscriptionStatusEnumExtension GetSubscriptionStatus(string subscriptionStatus)
        {
            if (!string.IsNullOrEmpty(subscriptionStatus))
            {
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.NotStarted)) return SubscriptionStatusEnumExtension.NotStarted;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.PendingFulfillmentStart)) return SubscriptionStatusEnumExtension.PendingFulfillmentStart;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.Subscribed)) return SubscriptionStatusEnumExtension.Subscribed;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.Unsubscribed)) return SubscriptionStatusEnumExtension.Unsubscribed;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.PendingActivation)) return SubscriptionStatusEnumExtension.PendingActivation;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.PendingUnsubscribe)) return SubscriptionStatusEnumExtension.PendingUnsubscribe;

                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.ActivationFailed)) return SubscriptionStatusEnumExtension.ActivationFailed;

                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.UnsubscribeFailed)) return SubscriptionStatusEnumExtension.UnsubscribeFailed;

                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.DeploymentPending)) return SubscriptionStatusEnumExtension.DeploymentPending;

                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.DeploymentSuccessful)) return SubscriptionStatusEnumExtension.DeploymentSuccessful;

                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.DeploymentFailed)) return SubscriptionStatusEnumExtension.DeploymentFailed;

                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.DeleteResourcePendign)) return SubscriptionStatusEnumExtension.DeleteResourcePendign;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.DeleteResourceFailed)) return SubscriptionStatusEnumExtension.DeleteResourceFailed;
                if (subscriptionStatus.Trim() == Convert.ToString(SubscriptionStatusEnumExtension.DeleteResourceSuccess)) return SubscriptionStatusEnumExtension.DeleteResourceSuccess;
            }
            return SubscriptionStatusEnumExtension.NotStarted;
        }

        /// <summary>
        /// Updates the subscription plan.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <returns></returns>
        public bool UpdateSubscriptionPlan(Guid subscriptionId, string planId)
        {
            if (subscriptionId != default && !string.IsNullOrEmpty(planId))
                SubscriptionRepository.UpdatePlanForSubscription(subscriptionId, planId);
            return false;
        }

        /// <summary>
        /// Updates the subscription quantity.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="quantity">The quantity identifier.</param>
        /// <returns></returns>
        public bool UpdateSubscriptionQuantity(Guid subscriptionId, int quantity)
        {
            if (subscriptionId != default && quantity > 0)
                SubscriptionRepository.UpdateQuantityForSubscription(subscriptionId, quantity);
            return false;
        }

        /// <summary>
        /// Adds the plan details for subscription.
        /// </summary>
        /// <param name="allPlanDetail">All plan detail.</param>
        public void AddPlanDetailsForSubscription(List<PlanDetailResultExtension> allPlanDetail)
        {
            foreach (var planDetail in allPlanDetail)
            {
                PlanRepository.Add(new Plans
                {
                    PlanId = planDetail.PlanId,
                    DisplayName = planDetail.PlanId,
                    Description = planDetail.DisplayName,
                    OfferId = planDetail.OfferId,
                    PlanGuid = planDetail.PlanGUID
                });
            }
        }

        /// <summary>
        /// Get the plan details for subscription.
        /// </summary>
        /// <returns></returns>
        public List<SaasKitModels.PlanDetailResult> GetAllSubscriptionPlans()
        {
            var allPlans = PlanRepository.Get();

            return (from plan in allPlans
                    select new SaasKitModels.PlanDetailResult()
                    {
                        Id = plan.Id,
                        PlanId = plan.PlanId,
                        DisplayName = plan.DisplayName,

                    }).ToList();
        }

        /// <summary>
        /// Get the plan details for subscription.
        /// </summary>
        /// <returns></returns>
        public List<SubscriptionParametersModel> GetSubscriptionsParametersById(Guid subscriptionId, Guid planId)
        {
            List<SubscriptionParametersModel> subscriptionParametersList = new List<SubscriptionParametersModel>();

            var subscriptionParameters = SubscriptionRepository.GetSubscriptionsParametersById(subscriptionId, planId);


            var serializedSubscription = JsonConvert.SerializeObject(subscriptionParameters);
            subscriptionParametersList = JsonConvert.DeserializeObject<List<SubscriptionParametersModel>>(serializedSubscription);

            return subscriptionParametersList;
        }

        /// <summary>
        /// Adds the plan details for subscription.
        /// </summary>
        /// <param name="allPlanDetail">All plan detail.</param>
        public void AddSubscriptionParameters(List<SubscriptionParametersModel> subscriptionParameters, int? currentUserId)
        {
            foreach (var parameters in subscriptionParameters)
            {
                SubscriptionRepository.AddSubscriptionParameters(new SubscriptionParametersOutput
                {
                    Id = parameters.Id,
                    PlanId = parameters.PlanId,
                    DisplayName = parameters.DisplayName,
                    PlanAttributeId = parameters.PlanAttributeId,
                    SubscriptionId = parameters.SubscriptionId,
                    OfferId = parameters.OfferId,
                    Value = parameters.Value,
                    UserId = currentUserId,
                    CreateDate = DateTime.Now


                }); ;
            }
        }


    }
}