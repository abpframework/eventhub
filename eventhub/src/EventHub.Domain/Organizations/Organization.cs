using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace EventHub.Organizations
{
    public class Organization : AggregateRoot<Guid>
    {
        public Guid OwnerUserId { get; set; }

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string Description { get; private set; }

        public string Website { get; set; }

        public string TwitterUsername { get; set; }

        public string GitHubUsername { get; set; }

        public string FacebookUsername { get; set; }

        public string InstagramUsername { get; set; }

        public string MediumUsername { get; set; }

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

        internal void SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), OrganizationConsts.MaxNameLength, OrganizationConsts.MinNameLength);
        }

        public void SetDisplayName(string displayName)
        {
            DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), OrganizationConsts.MaxDisplayNameLength, OrganizationConsts.MinDisplayNameLength);
        }

        public void SetDescription(string description)
        {
            Description = Check.NotNullOrWhiteSpace(description, nameof(description), OrganizationConsts.MaxDescriptionNameLength, OrganizationConsts.MinDescriptionNameLength);
        }
    }
}
