using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace EventHub.Organizations
{
    public class OrganizationAppService : EventHubAppService, IOrganizationAppService
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly OrganizationManager _organizationManager;

        public OrganizationAppService(
            IRepository<Organization, Guid> organizationRepository,
            OrganizationManager organizationManager)
        {
            _organizationRepository = organizationRepository;
            _organizationManager = organizationManager;
        }

        [Authorize]
        public async Task CreateAsync(CreateOrganizationDto input)
        {
            await _organizationManager.CreateAsync(
                CurrentUser.GetId(),
                input.Name,
                input.DisplayName,
                input.Description
            );
        }

        public async Task<PagedResultDto<OrganizationInListDto>> GetListAsync(PagedResultRequestDto input)
        {
            var organizationCount = await _organizationRepository.GetCountAsync();
            var organizations = await _organizationRepository.GetPagedListAsync(
                input.SkipCount,
                input.MaxResultCount,
                nameof(Organization.DisplayName)
            );

            return new PagedResultDto<OrganizationInListDto>(
                organizationCount,
                ObjectMapper.Map<List<Organization>, List<OrganizationInListDto>>(organizations)
            );
        }
    }
}
