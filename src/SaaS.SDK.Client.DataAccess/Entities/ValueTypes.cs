﻿using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class ValueTypes
    {
        public int ValueTypeId { get; set; }
        public string ValueType { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}