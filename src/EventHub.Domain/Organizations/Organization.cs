using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Timing;

namespace EventHub.Organizations
{
    public class Organization : FullAuditedAggregateRoot<Guid>
    {
        public Guid OwnerUserId { get; private set; }

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string Description { get; private set; }

        public string Website { get; set; }

        public string TwitterUsername { get; set; }

        public string GitHubUsername { get; set; }

        public string FacebookUsername { get; set; }

        public string InstagramUsername { get; set; }

        public string MediumUsername { get; set; }

        public int MemberCount { get; internal set; }

        public bool IsPremium { get; private set; }

        public DateTime? PremiumEndDate { get; private set; }

        public bool IsSendPremiumReminderEmail { get; set; }

        private Organization()
        {
        }

        internal Organization(
            Guid id,
            Guid ownerUserId,
            string name,
            string displayName,
            string description)
            : base(id)
        {
            OwnerUserId = ownerUserId;
            SetName(name);
            SetDisplayName(displayName);
            SetDescription(description);
        }

        internal Organization SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), OrganizationConsts.MaxNameLength, OrganizationConsts.MinNameLength);
            return this;
        }

        public Organization SetDisplayName(string displayName)
        {
            DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), OrganizationConsts.MaxDisplayNameLength, OrganizationConsts.MinDisplayNameLength);
            return this;
        }

        public Organization SetDescription(string description)
        {
            Description = Check.NotNullOrWhiteSpace(description, nameof(description), OrganizationConsts.MaxDescriptionNameLength, OrganizationConsts.MinDescriptionNameLength);
            return this;
        }

        internal Organization UpgradeToPremium(DateTime endDate)
        {
            SetPremiumStatus(true, endDate);

            return this;
        }
        
        public Organization DowngradeToPremium()
        {
            if (!IsPremium)
            {
                return this;
            }
            
            SetPremiumStatus(false);

            return this;
        }

        private Organization SetPremiumStatus(bool isPremium, DateTime? endDate = null)
        {
            if (isPremium)
            {
                Check.NotNull(endDate, nameof(endDate));
                IsPremium = true;
                PremiumEndDate = endDate;

                return this;
            }

            IsPremium = false;
            
            return this;
        }
    }
}
