using System;
using System.Threading.Tasks;
using EventHub.Admin.Organizations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Admin.Controllers.Organizations
{
    [Controller]
    [Area("eventhub-admin")]
    [ControllerName("Organization")]
    [Route("/api/eventhub/organizations")]
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

        [HttpPut]
        public Task<OrganizationProfileDto> UpdateAsync(Guid id, UpdateOrganizationDto input)
        {
            return _organizationAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _organizationAppService.DeleteAsync(id);
        }
    }
}