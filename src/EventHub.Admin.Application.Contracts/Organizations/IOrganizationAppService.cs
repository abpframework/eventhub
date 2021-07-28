using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Admin.Organizations
{
    public interface IOrganizationAppService : IApplicationService
    {
        Task<PagedResultDto<OrganizationInListDto>> GetListAsync(OrganizationListFilterDto input);

        Task<OrganizationProfileDto> GetAsync(Guid id);
        
        Task<OrganizationProfileDto> UpdateAsync(Guid id, UpdateOrganizationDto input);

        Task DeleteAsync(Guid id);
    }
}
