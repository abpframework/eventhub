using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Organizations
{
    public class OrganizationAppService : EventHubAppService, IOrganizationAppService
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;

        public OrganizationAppService(IRepository<Organization, Guid> organizationRepository)
        {
            _organizationRepository = organizationRepository;
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
