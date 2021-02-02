using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace EventHub.Organizations.Memberships
{
    public class OrganizationMembershipManagerTests : EventHubDomainTestBase
    {
        private readonly OrganizationMembershipManager _organizationMembershipManager;
        private readonly IRepository<OrganizationMembership, Guid> _organizationMembershipRepository;
        private readonly EventHubTestData _testData;

        public OrganizationMembershipManagerTests()
        {
            _organizationMembershipManager = GetRequiredService<OrganizationMembershipManager>();
            _organizationMembershipRepository = GetRequiredService<IRepository<OrganizationMembership, Guid>>();
            _testData = GetRequiredService<EventHubTestData>();
        }
        
        [Fact]
        public async Task Should_Join_To_An_Organization()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var user = await GetUserAsync(_testData.UserAdminId);
                var organization = await GetOrganizationAsync(_testData.OrganizationVolosoftName);
                await _organizationMembershipManager.JoinAsync(organization, user);
            });

            (await GetMembershipOrNull(_testData.OrganizationVolosoftId, _testData.UserAdminId))
                .ShouldNotBeNull();
        }

        private async Task<OrganizationMembership> GetMembershipOrNull(Guid organizationId, Guid userId)
        {
            return await WithUnitOfWorkAsync(async () =>
            {
                return await _organizationMembershipRepository.FirstOrDefaultAsync(
                    x => x.OrganizationId == organizationId && x.UserId == userId
                );
            });
        }
    }
}