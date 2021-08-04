using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Controllers.Organizations
{
    [RemoteService(Name = EventHubRemoteServiceConsts.RemoteServiceName)]
    [Area("eventhub")]
    [ControllerName("Organization")]
    [Route("api/eventhub/organization")]
    public class OrganizationController : AbpController, IOrganizationAppService
    {
        private readonly IOrganizationAppService _organizationAppService;

        public OrganizationController(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        [HttpPost]
        public async Task CreateAsync(CreateOrganizationDto input)
        {
            await _organizationAppService.CreateAsync(input);
        }

        [HttpGet]
        public async Task<PagedResultDto<OrganizationInListDto>> GetListAsync(OrganizationListFilterDto input)
        {
            return await _organizationAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<OrganizationProfileDto> GetProfileAsync(string name)
        {
            return await _organizationAppService.GetProfileAsync(name);
        }

        [HttpGet]
        [Route("by-user-id/{userId}")]
        public async Task<ListResultDto<OrganizationInListDto>> GetOrganizationsByUserIdAsync(Guid userId)
        {
            return await _organizationAppService.GetOrganizationsByUserIdAsync(userId);
        }

        [HttpGet]
        [Route("is-organization-owner/{organizationId}")]
        public async Task<bool> IsOrganizationOwnerAsync(Guid organizationId)
        {
            return await _organizationAppService.IsOrganizationOwnerAsync(organizationId);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateOrganizationDto input)
        {
            await _organizationAppService.UpdateAsync(id, input);
        }
    }
}