using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    public class SettingsModel
    {
        public string tempsmtppassword { get; set; }
        public List<AppConfigSettingsModel> appConfigSettings { get; set; }
    }
}
