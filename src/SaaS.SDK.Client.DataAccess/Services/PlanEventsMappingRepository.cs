using Microsoft.Marketplace.SaasKit.Client.DataAccess.Context;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Services
{
    public class PlanEventsMappingRepository : IPlanEventsMappingRepository
    {
        private readonly SaasKitContext Context;

        public PlanEventsMappingRepository(SaasKitContext context)
        {
            Context = context;
        }

        public string GetSuccessStateEmails(Guid PlanID)
        {
            var results = Context.PlanEventsMapping.Where(s => s.PlanId == PlanID);
            if (results.Count() == 0)
                return null;
            else
                return Context.PlanEventsMapping.Where(s => s.PlanId == PlanID).FirstOrDefault().SuccessStateEmails;
        }

        public string GetFailureStateEmails(Guid PlanID)
        {
            var results = Context.PlanEventsMapping.Where(s => s.PlanId == PlanID);
            if (results.Count() == 0)
                return null;
            else
                return Context.PlanEventsMapping.Where(s => s.PlanId == PlanID).FirstOrDefault().FailureStateEmails;
        }
    }
}
