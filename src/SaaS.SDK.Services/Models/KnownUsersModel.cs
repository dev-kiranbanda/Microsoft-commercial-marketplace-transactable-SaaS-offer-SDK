using System.Collections.Generic;

namespace Microsoft.Marketplace.SaaS.SDK.Services.Models
{
    public class KnownUsersModel
    {
        public int KnownUsersId { get; set; }

        public string KnownUsers { get; set; }

        public int RoleId { get; set; }

        public List<KnownUsersViewModel> KnownUsersList { get; set; }
    }
}