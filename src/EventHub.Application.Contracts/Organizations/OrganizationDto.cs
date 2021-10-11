using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Organizations
{
    public class OrganizationDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }
    }
}