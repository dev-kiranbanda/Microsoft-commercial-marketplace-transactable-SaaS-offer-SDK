﻿using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
using Microsoft.Marketplace.SaasKit.Client.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts
{
    public interface IPlanEventsMappingRepository
    {
        PlanEventsMapping GetPlanEventsMappingEmails(Guid PlanID,int eventID);
    }
}
