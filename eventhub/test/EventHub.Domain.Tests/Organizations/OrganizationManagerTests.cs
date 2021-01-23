using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Users;
using Xunit;

namespace EventHub.Organizations
{
    public class OrganizationManagerTests : EventHubDomainTestBase
    {
        private readonly OrganizationManager _organizationManager;
        private readonly EventHubTestData _testData;

        public OrganizationManagerTests()
        {
            _organizationManager = GetRequiredService<OrganizationManager>();
            _testData = GetRequiredService<EventHubTestData>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Organization()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var organization = await _organizationManager.CreateAsync(
                    CurrentUser.GetId(),
                    "test-org-1294",
                    "Test Display Name",
                    "Test description text that is valid and long enough!"
                );

                organization.Name.ShouldBe("test-org-1294");
                organization.DisplayName.ShouldBe("Test Display Name");
                organization.Description.ShouldBe("Test description text that is valid and long enough!");
            });
        }

        [Fact]
        public async Task Should_Not_Create_Organization_With_Existing_Name()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<BusinessException>(() =>
                    _organizationManager.CreateAsync(
                        CurrentUser.GetId(),
                        _testData.OrganizationVolosoftName,
                        "Test Display Name",
                        "Test description text that is valid and long enough!"
                    )
                );

                exception.Code.ShouldBe(EventHubErrorCodes.OrganizationNameAlreadyExists);
            });
        }
    }
}
