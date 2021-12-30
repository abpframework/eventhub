using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using EventHub.Organizations.Memberships;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace EventHub.Admin.Organizations.Memberships
{
    [Authorize(EventHubPermissions.Organizations.Memberships.Default)]
    public class OrganizationMembershipAppService : ApplicationService, IOrganizationMembershipAppService
    {
        private readonly IOrganizationMembershipRepository  _organizationMembershipRepository;
        private readonly IUserRepository _userRepository;
        
        public OrganizationMembershipAppService(
            IOrganizationMembershipRepository organizationMembershipRepository,
            IUserRepository userRepository)
        {
            _organizationMembershipRepository = organizationMembershipRepository;
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<OrganizationMemberDto>> GetListAsync(OrganizationMemberListFilterDto input)
        {
            IdentityUser user = null;
            if (!input.UserName.IsNullOrWhiteSpace())
            {
                input.UserName = input.UserName.ToLower();
                user = await _userRepository.SingleOrDefaultAsync(x => x.UserName.ToLower() == input.UserName);
                if (user is null)
                {
                    throw new BusinessException(EventHubErrorCodes.UserNotFound)
                        .WithData("UserName", input.UserName);
                }
            }

            var members = await _organizationMembershipRepository.GetMemberListAsync(input.OrganizationId, user?.Id, input.SkipCount, input.MaxResultCount);
            var totalCount = await _organizationMembershipRepository.GetCountAsync(input.OrganizationId, user?.Id);
            
            return new PagedResultDto<OrganizationMemberDto>(
                totalCount,
                ObjectMapper.Map<List<OrganizationMemberWithDetails>, List<OrganizationMemberDto>>(members)
            );
        }
    }
}