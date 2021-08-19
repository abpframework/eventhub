using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace EventHub.Organizations.Memberships
{
    public class OrganizationMembershipAppService : EventHubAppService, IOrganizationMembershipAppService 
    {
        private readonly OrganizationMembershipManager _organizationMembershipManager;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IOrganizationMembershipRepository  _organizationMembershipsRepository;

        public OrganizationMembershipAppService(
            OrganizationMembershipManager organizationMembershipManager,
            IUserRepository userRepository, 
            IRepository<Organization, Guid> organizationRepository,
            IOrganizationMembershipRepository organizationMembershipsRepository)
        {
            _organizationMembershipManager = organizationMembershipManager;
            _userRepository = userRepository;
            _organizationRepository = organizationRepository;
            _organizationMembershipsRepository = organizationMembershipsRepository;
        }
        
        [Authorize]
        public async Task JoinAsync(Guid organizationId)
        {
            await _organizationMembershipManager.JoinAsync(
                await _organizationRepository.GetAsync(organizationId),
                await _userRepository.GetAsync(CurrentUser.GetId())
            );
        }
        
        [Authorize]
        public async Task LeaveAsync(Guid organizationId)
        {
            await _organizationMembershipsRepository.DeleteAsync(
                x => x.OrganizationId == organizationId && x.UserId == CurrentUser.GetId()
            );
        }

        [Authorize]
        public async Task<bool> IsJoinedAsync(Guid organizationId)
        {
            return await _organizationMembershipManager.IsJoinedAsync(
                await _organizationRepository.GetAsync(organizationId),
                await _userRepository.GetAsync(CurrentUser.GetId())
            );
        }

        public async Task<PagedResultDto<OrganizationMemberDto>> GetMembersAsync(OrganizationMemberListFilterDto input)
        {
            var organizationMembershipsQueryable = await _organizationMembershipsRepository.GetQueryableAsync();
            var userQueryable = await _userRepository.GetQueryableAsync();

            var query = from organizationMembership in organizationMembershipsQueryable
                join user in userQueryable on organizationMembership.UserId equals user.Id
                where organizationMembership.OrganizationId == input.OrganizationId
                orderby organizationMembership.CreationTime descending
                select user;

            var totalCount = await AsyncExecuter.CountAsync(query);
            
            var users = await AsyncExecuter.ToListAsync(query.PageBy(input));

            return new PagedResultDto<OrganizationMemberDto>(
                totalCount,
                ObjectMapper.Map<List<IdentityUser>, List<OrganizationMemberDto>>(users)
            );
        }
    }
}