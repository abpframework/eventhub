using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Organizations
{
    public class OrganizationInListDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public int MemberCount { get; set; }
    }
}