using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Users;
using Xunit;

namespace EventHub.Organizations
{
    public class OrganizationAppServiceTests : EventHubApplicationTestBase
    {
        private readonly IOrganizationAppService _organizationAppService;
        private readonly EventHubTestData _testData;
        private ICurrentUser _currentUser;
        
        public OrganizationAppServiceTests()
        {
            _organizationAppService = GetRequiredService<IOrganizationAppService>();
            _testData = GetRequiredService<EventHubTestData>();
        }
        
        protected override void AfterAddApplication(IServiceCollection services)
        {
            _currentUser = Substitute.For<ICurrentUser>();
            services.AddSingleton(_currentUser);
        }

        [Fact]
        public async Task Should_Create_A_Valid_Organization()
        {
            Login(_testData.UserAdminId);
            
            await _organizationAppService.CreateAsync(
                new CreateOrganizationDto
                {
                    Name = "test-org-8374",
                    DisplayName = "Test Display Name",
                    Description = "Test description text that is valid and long enough!"
                }
            );

            var organization = await GetOrganizationOrNullAsync("test-org-8374");
            organization.Name.ShouldBe("test-org-8374");
            organization.DisplayName.ShouldBe("Test Display Name");
            organization.Description.ShouldBe("Test description text that is valid and long enough!");
        }

        [Fact]
        public async Task Should_Not_Create_Organization_With_Existing_Name()
        {
            Login(_testData.UserAdminId);
            
            var exception = await Assert.ThrowsAsync<BusinessException>(() =>
                _organizationAppService.CreateAsync(
                    new CreateOrganizationDto
                    {
                        Name = _testData.OrganizationVolosoftName,
                        DisplayName = "Test Display Name",
                        Description = "Test description text that is valid and long enough!"
                    }
                )
            );

            exception.Code.ShouldBe(EventHubErrorCodes.OrganizationNameAlreadyExists);
        }

        [Fact]
        public async Task Should_Get_List_Of_Organizations()
        {
            var result = await _organizationAppService.GetListAsync(new OrganizationListFilterDto());
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
            result.Items.ShouldContain(o => o.Name == _testData.OrganizationVolosoftName && o.Id == _testData.OrganizationVolosoftId);
            result.Items.ShouldContain(o => o.Name == _testData.OrganizationDotnetEuropeName && o.Id == _testData.OrganizationDotnetEuropeId);
        }

        [Fact]
        public async Task Should_Get_An_Organization_Profile()
        {
            var result = await _organizationAppService.GetProfileAsync(_testData.OrganizationVolosoftName);
            result.Id.ShouldBe(_testData.OrganizationVolosoftId);
            result.Name.ShouldBe(_testData.OrganizationVolosoftName);
        }
        
        [Fact]
        public async Task Should_Get_List_Of_Organizations_By_UserId()
        {
            var result = await _organizationAppService.GetOrganizationsByUserIdAsync(_testData.UserAdminId);
            result.Items.Count.ShouldBeGreaterThanOrEqualTo(1);
            result.Items.ShouldContain(o => o.Name == _testData.OrganizationVolosoftName && o.Id == _testData.OrganizationVolosoftId);
        }

        [Fact]
        public async Task Should_Update_Organization_If_User_Is_Owner()
        {
            Login(_testData.UserAdminId);
            
            await _organizationAppService.UpdateAsync(
                _testData.OrganizationVolosoftId,
                new UpdateOrganizationDto
                {
                    DisplayName = "VOLOSOFT",
                    Description = "Test description text that is valid and long enough for updating!",
                    Website = "https://volosoft.com/",
                    MediumUsername = "volosoft"
                });

            var organization = await GetOrganizationOrNullAsync(_testData.OrganizationVolosoftName);
            organization.ShouldNotBeNull();
            organization.DisplayName.ShouldBe("VOLOSOFT");
            organization.Description.ShouldBe("Test description text that is valid and long enough for updating!");
        }

        [Fact]
        public async Task Should_Not_Update_Organization_If_User_Not_Owner()
        {
            Login(_testData.UserAdminId);

            var exception = await Assert.ThrowsAsync<AbpAuthorizationException>(() =>
                _organizationAppService.UpdateAsync(
                    _testData.OrganizationDotnetEuropeId,
                    new UpdateOrganizationDto
                    {
                        DisplayName = "Dotnet Europe (DE)",
                        Description = "Test description text that is valid and long enough for updating!",
                        Website = "https://dotnet-europe.com/"
                    })
            );
            
            exception.ShouldNotBeNull();
        }
        
        private void Login(Guid userId)
        {
            _currentUser.Id.Returns(userId);
            _currentUser.IsAuthenticated.Returns(true);
        }
    }
}
