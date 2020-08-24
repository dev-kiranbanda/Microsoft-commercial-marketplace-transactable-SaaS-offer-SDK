

namespace Microsoft.Marketplace.SaaS.SDK.Services.Services
{
    using Microsoft.Marketplace.SaaS.SDK.Services.Models;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Contracts;
    using Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities;
    using System.Collections.Generic;

    public class KnownUserService
    {

        /// <summary>
        /// The user repository.
        /// </summary>
        private IKnownUsersRepository knowUsersRepository;

        public KnownUserService(IKnownUsersRepository knowUsersRepository)
        {
            this.knowUsersRepository = knowUsersRepository;
        }

        public List<KnownUsersViewModel> GetAllKnownUsers()
        {
            List<KnownUsersViewModel> knownUsersList = new List<KnownUsersViewModel>();
            var allKnownUsersData = this.knowUsersRepository.Get();
            foreach (var item in allKnownUsersData)
            {
                KnownUsersViewModel knownUser = new KnownUsersViewModel();
                knownUser.KnownUsersId = item.Id;
                knownUser.KnownUsers = item.UserEmail;
                knownUser.RoleId = item.RoleId;
                knownUsersList.Add(knownUser);
            }

            return knownUsersList;

        }

        /// <summary>
        /// Adds the partner detail.
        /// </summary>
        /// <param name="partnerDetailViewModel">The partner detail view model.</param>
        /// <returns> User id.</returns>
        public int AddKnownUser(KnownUsersModel knownUserModel)
        {
            if (knownUserModel != null && !string.IsNullOrEmpty(knownUserModel.KnownUsers))
            {
                KnownUsers knownuser = new KnownUsers()
                {
                    UserEmail = knownUserModel.KnownUsers,
                    RoleId = 1,
                };
                this.knowUsersRepository.Save(knownuser);
            }

            return knownUserModel.KnownUsersId;
        }

        /// <summary>
        /// Adds the partner detail.
        /// </summary>
        /// <param name="partnerDetailViewModel">The partner detail view model.</param>
        /// <returns> User id.</returns>
        public int RemoveKnownUser(KnownUsersModel knownUserModel)
        {
            if (knownUserModel != null && !string.IsNullOrEmpty(knownUserModel.KnownUsers))
            {
                KnownUsers knownuser = new KnownUsers()
                {
                    Id = knownUserModel.KnownUsersId,
                    UserEmail = knownUserModel.KnownUsers
                };
                this.knowUsersRepository.Remove(knownuser);
            }

            return knownUserModel.KnownUsersId;
        }
    }
}
