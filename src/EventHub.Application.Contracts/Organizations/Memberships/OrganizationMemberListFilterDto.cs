using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Organizations.Memberships
{
    public class OrganizationMemberListFilterDto : PagedResultRequestDto
    {
        public Guid OrganizationId { get; set; }
    }
}