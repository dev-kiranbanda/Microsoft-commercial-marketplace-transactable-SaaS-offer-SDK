﻿using Microsoft.Marketplace.Saas.Web.Models;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaasKit.Client.Services
{
    public class PlansService
    {
        public IPlansRepository plansRepository;

        public IOfferAttributesRepository offerAttributeRepository;

        public IOffersRepository offerRepository;
        public PlansService(IPlansRepository plansRepository, IOfferAttributesRepository offerAttributeRepository, IOffersRepository offerRepository)
        {
            this.offerRepository = offerRepository;
            this.plansRepository = plansRepository;
            this.offerAttributeRepository = offerAttributeRepository;
        }

        public List<PlansModel> GetPlans()
        {

            List<PlansModel> plansList = new List<PlansModel>();
            var allPlansData = this.plansRepository.GetPlansByUser();
            foreach (var item in allPlansData)
            {
                PlansModel Plans = new PlansModel();
                Plans.planId = item.PlanId;
                Plans.DisplayName = item.DisplayName;
                Plans.Description = item.Description;
                Plans.IsPerUser = item.IsPerUser ?? false;
                Plans.IsmeteringSupported = item.IsmeteringSupported ?? false;
                Plans.offerID = item.OfferId;
                Plans.DeployToCustomerSubscription = item.DeployToCustomerSubscription;
                Plans.PlanGUID = item.PlanGuid;
                plansList.Add(Plans);
            }

            var offerDetails = this.offerRepository.Get();
            plansList.ForEach(x => x.OfferName = offerDetails.Where(s => s.OfferGuid == x.offerID).FirstOrDefault().OfferName);

            return plansList;
        }

        public PlansModel GetPlanDetailByPlanGuId(Guid planGuId)
        {
            var existingPlan = this.plansRepository.GetPlanDetailByPlanGuId(planGuId);
            var planAttributes = this.plansRepository.GetPlanAttributesByPlanGuId(planGuId, existingPlan.OfferId);
            var planEvents = this.plansRepository.GetPlanEventsByPlanGuId(planGuId, existingPlan.OfferId);
            var offerDetails = this.offerRepository.GetOfferDetailByOfferId(existingPlan.OfferId);
            //var offerAttributes = this.offerAttributeRepository.GetOfferAttributeDetailByOfferId();
            //var activeAttribute = offerAttributes.Where(s => s.Isactive == true);

            PlansModel plan = new PlansModel
            {
                Id = existingPlan.Id,
                planId = existingPlan.PlanId,
                IsPerUser = existingPlan.IsPerUser ?? false,
                IsmeteringSupported = existingPlan.IsmeteringSupported ?? false,
                offerID = existingPlan.OfferId,
                DisplayName = existingPlan.DisplayName,
                Description = existingPlan.Description,
                PlanGUID = existingPlan.PlanGuid,
                OfferName = offerDetails.OfferName
            };

            plan.PlanAttributes = new List<PlanAttributesModel>();

            foreach (var attribute in planAttributes)
            {
                PlanAttributesModel planAttributesmodel = new PlanAttributesModel()
                {
                    OfferAttributeId = attribute.OfferAttributeId,
                    PlanAttributeId = attribute.PlanAttributeId,
                    PlanId = existingPlan.PlanGuid,
                    DisplayName = attribute.DisplayName,
                    IsEnabled = attribute.IsEnabled
                };
                plan.PlanAttributes.Add(planAttributesmodel);
            }

            plan.PlanEvents = new List<PlanEventsModel>();

            foreach (var events in planEvents)
            {
                PlanEventsModel planEventsModel = new PlanEventsModel()
                {
                    Id = events.Id,
                    PlanId = events.PlanId,
                    Isactive = events.Isactive,
                    SuccessStateEmails = events.SuccessStateEmails,
                    FailureStateEmails = events.FailureStateEmails,
                    EventName = events.EventsName,
                    EventId = events.EventId,
                    CopyToCustomer = events.CopyToCustomer
                };
                plan.PlanEvents.Add(planEventsModel);
            }
            return plan;
        }

        public int? SavePlanMeteringParameter(PlansModel plan)
        {
            var existingPlan = this.plansRepository.GetPlanDetailByPlanGuId(plan.PlanGUID);
            existingPlan.IsPerUser = plan.IsPerUser;
            existingPlan.IsmeteringSupported = plan.IsmeteringSupported;
            this.plansRepository.Add(existingPlan);
            return null;
        }

        public int? SavePlanAttributes(PlanAttributesModel planAttributes)
        {
            if (planAttributes != null)
            {
                PlanAttributeMapping attribute = new PlanAttributeMapping();
                attribute.OfferAttributeId = planAttributes.OfferAttributeId;
                attribute.IsEnabled = planAttributes.IsEnabled;
                attribute.PlanId = planAttributes.PlanId;
                attribute.UserId = planAttributes.UserId;
                attribute.PlanAttributeId = planAttributes.PlanAttributeId;
                attribute.CreateDate = DateTime.Now;

                var planEventsId = this.plansRepository.AddPlanAttributes(attribute);
                return planEventsId;
            }
            return null;
        }


        public int? SavePlanEvents(PlanEventsModel planEvents)
        {
            if (planEvents != null)
            {
                PlanEventsMapping events = new PlanEventsMapping();
                events.Id = planEvents.Id;
                events.Isactive = planEvents.Isactive;
                events.PlanId = planEvents.PlanId;
                events.SuccessStateEmails = planEvents.SuccessStateEmails;
                events.FailureStateEmails = planEvents.FailureStateEmails;
                events.EventId = planEvents.EventId;
                events.UserId = planEvents.UserId;
                events.CreateDate = DateTime.Now;
                events.CopytoCustomer = planEvents.CopyToCustomer;
                var planEventsId = this.plansRepository.AddPlanEvents(events);
                return planEventsId;
            }
            return null;
        }

    }
}
