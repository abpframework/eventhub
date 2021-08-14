using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Organizations.Memberships
{
    public class OrganizationMemberListFilterDto : PagedResultRequestDto
    {
        public Guid? OrganizationId { get; set; }
        
        public string UserName { get; set; }
    }
}