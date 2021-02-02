using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace EventHub.Organizations.Memberships
{
    public class OrganizationMembershipAppServiceTests : EventHubApplicationTestBase
    {
        private readonly IOrganizationMembershipAppService _organizationMembershipAppService;
        private readonly IRepository<OrganizationMembership, Guid> _organizationMembershipRepository;
        private readonly EventHubTestData _testData;
        private readonly ICurrentUser _currentUser;

        public OrganizationMembershipAppServiceTests()
        {
            _organizationMembershipAppService = GetRequiredService<IOrganizationMembershipAppService>();
            _organizationMembershipRepository = GetRequiredService<IRepository<OrganizationMembership, Guid>>();
            _testData = GetRequiredService<EventHubTestData>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }
        
        [Fact]
        public async Task Should_Join_To_An_Organization()
        {
            await _organizationMembershipAppService.JoinAsync(
                _testData.OrganizationVolosoftId
            );

            (await GetMembershipOrNull(
                _testData.OrganizationVolosoftId,
                _currentUser.GetId()
            )).ShouldNotBeNull();
        }
        
        [Fact]
        public async Task Should_Leave_From_An_Organization()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await _organizationMembershipRepository.InsertAsync(
                    new OrganizationMembership(
                        Guid.NewGuid(),
                        _testData.OrganizationDotnetEuropeId,
                        _currentUser.GetId()
                    )
                );
            });

            await _organizationMembershipAppService.LeaveAsync(
                _testData.OrganizationDotnetEuropeId
            );

            (await GetMembershipOrNull(
                    _testData.OrganizationDotnetEuropeId,
                    _currentUser.GetId())
                ).ShouldBeNull();
        }
        
        [Fact]
        public async Task Should_Get_List_Of_Members()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await _organizationMembershipRepository.InsertAsync(
                    new OrganizationMembership(
                        Guid.NewGuid(),
                        _testData.OrganizationVolosoftId,
                        _testData.UserAdminId
                    )
                );

                await _organizationMembershipRepository.InsertAsync(
                    new OrganizationMembership(
                        Guid.NewGuid(),
                        _testData.OrganizationVolosoftId,
                        _testData.UserJohnId
                    )
                );
            });

            var result = await _organizationMembershipAppService.GetMembersAsync(_testData.OrganizationVolosoftId);

            result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
            result.Items.ShouldContain(x => x.Id == _testData.UserAdminId);
            result.Items.ShouldContain(x => x.Id == _testData.UserJohnId);
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