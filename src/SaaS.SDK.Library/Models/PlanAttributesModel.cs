﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Marketplace.SaaS.SDK.Library.Models
{
    public class PlanAttributesModel
    {
        public int PlanAttributeId { get; set; }
        public Guid PlanId { get; set; }
        public int OfferAttributeId { get; set; }
        public string DisplayName { get; set; }
        public bool IsEnabled { get; set; }
        public int UserId { get; set; }

        public string Type { get; set; }
    }
}