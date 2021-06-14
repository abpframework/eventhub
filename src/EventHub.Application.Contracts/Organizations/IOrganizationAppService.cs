using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Organizations
{
    public interface IOrganizationAppService : IApplicationService
    {
        Task CreateAsync(CreateOrganizationDto input);

        Task<PagedResultDto<OrganizationInListDto>> GetListAsync(OrganizationListFilterDto input);

        Task<OrganizationProfileDto> GetProfileAsync(string name);

        Task<ListResultDto<OrganizationInListDto>> GetOrganizationsByUserIdAsync(Guid userId);

        Task<bool> IsOrganizationOwnerAsync(Guid organizationId);

        Task UpdateAsync(Guid id, UpdateOrganizationDto input);
    }
}
