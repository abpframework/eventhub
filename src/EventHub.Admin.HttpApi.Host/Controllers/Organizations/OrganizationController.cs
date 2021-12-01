using System;
using System.Threading.Tasks;
using EventHub.Admin.Organizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace EventHub.Admin.Controllers.Organizations
{
    [RemoteService(Name = EventHubAdminRemoteServiceConsts.RemoteServiceName)]
    [Controller]
    [Area("eventhub-admin")]
    [ControllerName("Organization")]
    [Route("/api/eventhub/admin/organization")]
    public class OrganizationController : AbpController, IOrganizationAppService
    {
        private readonly IOrganizationAppService _organizationAppService;
        
        public OrganizationController(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<OrganizationInListDto>> GetListAsync(OrganizationListFilterDto input)
        {
            return _organizationAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public Task<OrganizationProfileDto> GetAsync(Guid id)
        {
            return _organizationAppService.GetAsync(id);
        }
        
        [HttpGet]
        [Route("by-name/{name}")]
        public Task<OrganizationProfileDto> GetByNameAsync(string name)
        {
            return _organizationAppService.GetByNameAsync(name);
        }

        [HttpPut]
        public Task<OrganizationProfileDto> UpdateAsync(Guid id, [FromForm] UpdateOrganizationDto input)
        {
            return _organizationAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _organizationAppService.DeleteAsync(id);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("cover-image/{id}")]
        public async Task<IRemoteStreamContent> GetCoverImageAsync(Guid id)
        {
            var remoteStreamContent = await _organizationAppService.GetCoverImageAsync(id);
            if (remoteStreamContent is null)
            {
                return null;
            }
            
            Response.Headers.Add("Accept-Ranges", "bytes");
            Response.ContentType = remoteStreamContent.ContentType;

            return remoteStreamContent;
        }
    }
}