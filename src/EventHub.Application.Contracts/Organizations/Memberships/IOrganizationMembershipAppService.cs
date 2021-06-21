using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Organizations.Memberships
{
    public interface IOrganizationMembershipAppService : IApplicationService
    {
        Task JoinAsync(Guid organizationId);

        Task LeaveAsync(Guid organizationId);

        Task<bool> IsJoinedAsync(Guid organizationId);

        Task<PagedResultDto<OrganizationMemberDto>> GetMembersAsync(OrganizationMemberListFilterDto input);
    }
}