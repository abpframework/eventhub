using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.VirtualFileSystem;

namespace EventHub.Controllers.Organizations
{
    [RemoteService(Name = EventHubRemoteServiceConsts.RemoteServiceName)]
    [Area("eventhub")]
    [ControllerName("Organization")]
    [Route("api/eventhub/organization")]
    public class OrganizationController : EventHubController, IOrganizationAppService
    {
        private readonly IOrganizationAppService _organizationAppService;
        private readonly IVirtualFileProvider _virtualFileProvider;

        public OrganizationController(
            IOrganizationAppService organizationAppService, 
            IVirtualFileProvider virtualFileProvider)
        {
            _organizationAppService = organizationAppService;
            _virtualFileProvider = virtualFileProvider;
        }

        [HttpPost]
        public async Task<OrganizationDto> CreateAsync([FromForm] CreateOrganizationDto input)
        {
            return await _organizationAppService.CreateAsync(input);
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
        public async Task UpdateAsync(Guid id, [FromForm] UpdateOrganizationDto input)
        {
            await _organizationAppService.UpdateAsync(id, input);
        }

        [HttpGet]
        [Route("profile-picture/{id}")]
        public async Task<IRemoteStreamContent> GetProfilePictureAsync(Guid id)
        {
            var remoteStreamContent = await _organizationAppService.GetProfilePictureAsync(id);

            if (remoteStreamContent is null)
            {
                var stream = _virtualFileProvider.GetFileInfo("/Images/eh-organization.png").CreateReadStream();
                remoteStreamContent = new RemoteStreamContent(stream);
                await stream.FlushAsync();
            }
            
            Response.Headers.Add("Accept-Ranges", "bytes");
            Response.ContentType = remoteStreamContent.ContentType;

            return remoteStreamContent;
        }

        [HttpGet]
        [Route("plan-infos")]
        public async Task<List<PlanInfoDefinitionDto>> GetPlanInfosAsync()
        {
            return await _organizationAppService.GetPlanInfosAsync();
        }
    }
}
