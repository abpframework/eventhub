using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Organizations
{
    public class OrganizationProfileDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string OwnerUserName { get; set; }

        public string OwnerEmail { get; set; }

        public string Description { get; set; }

        public string Website { get; set; }

        public string TwitterUsername { get; set; }

        public string GitHubUsername { get; set; }

        public string FacebookUsername { get; set; }

        public string InstagramUsername { get; set; }

        public string MediumUsername { get; set; }
    }
}