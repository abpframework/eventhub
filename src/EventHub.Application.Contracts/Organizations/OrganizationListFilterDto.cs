using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Organizations
{
    public class OrganizationListFilterDto : PagedResultRequestDto
    {
        public Guid? RegisteredUserId { get; set; }
    }
}