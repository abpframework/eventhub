using System;
using System.Threading.Tasks;
using EventHub.Organizations.Memberships;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace EventHub.Controllers.Organizations.Memberships
{
    [RemoteService(Name = EventHubRemoteServiceConsts.RemoteServiceName)]
    [Area("eventhub")]
    [ControllerName("OrganizationMembership")]
    [Route("api/eventhub/organization-membership")]
    public class OrganizationMembershipController : EventHubController, IOrganizationMembershipAppService
    {
        private readonly IOrganizationMembershipAppService _organizationMembershipAppService;

        public OrganizationMembershipController(IOrganizationMembershipAppService organizationMembershipAppService)
        {
            _organizationMembershipAppService = organizationMembershipAppService;
        }

        [HttpPut]
        [Route("join/{organizationId}")]
        public async Task JoinAsync(Guid organizationId)
        {
            await _organizationMembershipAppService.JoinAsync(organizationId);
        }

        [HttpPut]
        [Route("leave/{organizationId}")]
        public async Task LeaveAsync(Guid organizationId)
        {
            await _organizationMembershipAppService.LeaveAsync(organizationId);

        }

        [HttpGet]
        [Route("is-joined/{organizationId}")]
        public async Task<bool> IsJoinedAsync(Guid organizationId)
        {
            return await _organizationMembershipAppService.IsJoinedAsync(organizationId);
        }

        [HttpGet]
        public async Task<PagedResultDto<OrganizationMemberDto>> GetMembersAsync(OrganizationMemberListFilterDto input)
        {
            return await _organizationMembershipAppService.GetMembersAsync(input);
        }
    }
}
