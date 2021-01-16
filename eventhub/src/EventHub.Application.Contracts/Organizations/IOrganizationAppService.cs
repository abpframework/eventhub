using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Organizations
{
    public interface IOrganizationAppService : IApplicationService
    {
        Task<PagedResultDto<OrganizationInListDto>> GetListAsync(PagedResultRequestDto input);
    }
}
