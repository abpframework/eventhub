using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Organizations
{
    public interface IOrganizationAppService : IApplicationService
    {
        Task CreateAsync(CreateOrganizationDto input);

        Task<PagedResultDto<OrganizationInListDto>> GetListAsync(PagedResultRequestDto input);

        Task<OrganizationProfileDto> GetProfileAsync(string name);
    }
}
