using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Admin.Organizations.Memberships
{
    public interface IOrganizationMembershipAppService : IApplicationService
    {
        Task<PagedResultDto<OrganizationMemberDto>> GetListAsync(OrganizationMemberListFilterDto input);
    }
}