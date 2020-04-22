﻿using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using Microsoft.Marketplace.SaasKit.Client.Models;
using System.Collections;
using Commons.Collections;
using NVelocity.App;
using NVelocity;
using System.IO;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using Microsoft.Marketplace.SaasKit.Models;
using System;
using System.Linq;

namespace Microsoft.Marketplace.SaasKit.Web.Services
{
    public class TemplateService
    {
        public static string ProcessTemplate(SubscriptionResultExtension Subscription, IEmailTemplateRepository emailTemplateRepository, IApplicationConfigRepository applicationConfigRepository, string planEvent, SubscriptionStatusEnum oldValue, string newValue)
        {
            string body = string.Empty;
            if (planEvent == "failure")
            {
                body = emailTemplateRepository.GetTemplateBody(planEvent);
            }
            else
            {
                body = emailTemplateRepository.GetTemplateBody(Subscription.SaasSubscriptionStatus.ToString());
            }
            string applicationName = applicationConfigRepository.GetValuefromApplicationConfig("ApplicationName");
            string emailLogo = applicationConfigRepository.GetValuefromApplicationConfig("EmailLogo");
            string contactURL = applicationConfigRepository.GetValuefromApplicationConfig("ContactURL");
            string saasAppURL = applicationConfigRepository.GetValuefromApplicationConfig("SaasAppURL");
            
                

            Hashtable hashTable = new Hashtable();
            hashTable.Add("ApplicationName", applicationName ?? "");
            hashTable.Add("CustomerEmailAddress", Subscription.CustomerEmailAddress ?? "");
            hashTable.Add("CustomerName", Subscription.CustomerName ?? "");
            hashTable.Add("Id", Convert.ToString(Subscription.Id) ?? "");
            hashTable.Add("SubscriptionName", Subscription.Name ?? "");
            hashTable.Add("SaasSubscriptionStatus", Convert.ToString(Subscription.SaasSubscriptionStatus) ?? "");
            hashTable.Add("oldValue", Convert.ToString(oldValue) ?? "");
            hashTable.Add("newValue", newValue ?? "");
            hashTable.Add("OfferID", Subscription.OfferId ?? "");
            hashTable.Add("Plan", Subscription.PlanId ?? "");
            hashTable.Add("PurchaserEmail", Subscription.Purchaser.EmailId ?? "");
            hashTable.Add("PurchaserTenant", Convert.ToString(Subscription.Purchaser.TenantId) ?? "");
            hashTable.Add("EmailLogo", emailLogo ?? "");
            hashTable.Add("ContactURL", contactURL ?? "");
            hashTable.Add("SaasAppURL", saasAppURL ?? "");
            
                

            ExtendedProperties p = new ExtendedProperties();

            VelocityEngine v = new VelocityEngine();
            v.Init(p);

            VelocityContext context = new VelocityContext(hashTable);
            IList list;
            if (Subscription.SubscriptionParameters != null && Subscription.SubscriptionParameters.Count > 0)
            {
                list = Subscription.SubscriptionParameters.Where(s => s.Type.ToLower() == "input").ToList();
                if (list.Count > 0)
                    context.Put("parms", list);
            }
            StringWriter writer = new StringWriter();
            v.Evaluate(context, writer, string.Empty, body);
            return writer.ToString();
        }
    }
}
