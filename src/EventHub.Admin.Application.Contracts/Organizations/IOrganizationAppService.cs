using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace EventHub.Admin.Organizations
{
    public interface IOrganizationAppService : IApplicationService
    {
        Task<PagedResultDto<OrganizationInListDto>> GetListAsync(OrganizationListFilterDto input);

        Task<OrganizationProfileDto> GetAsync(Guid id);
        
        Task<OrganizationProfileDto> GetByNameAsync(string name);
        
        Task<OrganizationProfileDto> UpdateAsync(Guid id, UpdateOrganizationDto input);

        Task DeleteAsync(Guid id);

        Task<IRemoteStreamContent> GetCoverImageAsync(Guid id);
    }
}
