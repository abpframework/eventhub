using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace EventHub.Organizations.Memberships
{
    public class OrganizationMembershipManager : DomainService
    {
        private readonly IRepository<OrganizationMembership, Guid> _organizationMembershipsRepository;

        public OrganizationMembershipManager(IRepository<OrganizationMembership, Guid> organizationMembershipsRepository)
        {
            _organizationMembershipsRepository = organizationMembershipsRepository;
        }
        
        public async Task JoinAsync(
            Organization organization,
            IdentityUser user)
        {
            if (await IsJoinedAsync(organization, user))
            {
                return;
            }

            await _organizationMembershipsRepository.InsertAsync(
                new OrganizationMembership(
                    GuidGenerator.Create(),
                    organization.Id,
                    user.Id
                )
            );
        }

        public async Task<bool> IsJoinedAsync(
            Organization organization,
            IdentityUser user)
        {
            return await _organizationMembershipsRepository
                .AnyAsync(x => x.OrganizationId == organization.Id && x.UserId == user.Id);
        }
    }
}