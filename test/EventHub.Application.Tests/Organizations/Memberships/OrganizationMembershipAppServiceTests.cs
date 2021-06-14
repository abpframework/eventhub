using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
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
        private ICurrentUser _currentUser;

        public OrganizationMembershipAppServiceTests()
        {
            _organizationMembershipAppService = GetRequiredService<IOrganizationMembershipAppService>();
            _organizationMembershipRepository = GetRequiredService<IRepository<OrganizationMembership, Guid>>();
            _testData = GetRequiredService<EventHubTestData>();
        }
        
        protected override void AfterAddApplication(IServiceCollection services)
        {
            _currentUser = Substitute.For<ICurrentUser>();
            services.AddSingleton(_currentUser);
        }
        
        [Fact]
        public async Task Should_Join_To_An_Organization()
        {
            Login(_testData.UserAdminId);

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
            Login(_testData.UserAdminId);

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
        public async Task Should_True_To_A_Joined_In_Organization()
        {
            Login(_testData.UserAdminId);

            await _organizationMembershipAppService.JoinAsync(_testData.OrganizationDotnetEuropeId);
            var result = await _organizationMembershipAppService.IsJoinedAsync(
                _testData.OrganizationDotnetEuropeId
            );

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_False_To_A_Not_Joined_In_Organization()
        {
            Login(_testData.UserAdminId);

            await _organizationMembershipAppService.LeaveAsync(_testData.OrganizationDotnetEuropeId);

            var result = await _organizationMembershipAppService.IsJoinedAsync(
                _testData.OrganizationDotnetEuropeId
            );

            result.ShouldBeFalse();
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

            var result = await _organizationMembershipAppService.GetMembersAsync(new OrganizationMemberListFilterDto{OrganizationId = _testData.OrganizationVolosoftId});

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
        
        private void Login(Guid userId)
        {
            _currentUser.Id.Returns(userId);
            _currentUser.IsAuthenticated.Returns(true);
        }
    }
}