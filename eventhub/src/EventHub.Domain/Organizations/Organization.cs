using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace EventHub.Organizations
{
    public class Organization : AggregateRoot<Guid>
    {
        public string Name { get; private set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Website { get; set; }

        public string TwitterUsername { get; set; }

        public string GitHubUsername { get; set; }

        public string FacebookUsername { get; set; }

        public string InstagramUsername { get; set; }

        public string MediumUsername { get; set; }

        private Organization()
        {

        }

        public Organization(
            Guid id,
            string name,
            string displayName)
            : base(id)
        {
            SetName(name);
            SetDisplayName(displayName);
        }

        internal void SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), OrganizationConsts.MaxNameLength, OrganizationConsts.MinNameLength);
        }

        public void SetDisplayName(string displayName)
        {
            DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName));
        }
    }
}
