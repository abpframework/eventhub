using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Organizations
{
    public class OrganizationInListDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public OrganizationPlanType PlanType { get; set; }
    }
}
