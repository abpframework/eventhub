using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace EventHub.Organizations
{
    public class OrganizationAppServiceTests : EventHubApplicationTestBase
    {
        private readonly IOrganizationAppService _organizationAppService;
        private readonly EventHubTestData _testData;

        public OrganizationAppServiceTests()
        {
            _organizationAppService = GetRequiredService<IOrganizationAppService>();
            _testData = GetRequiredService<EventHubTestData>();
        }

        [Fact]
        public async Task Should_Create_A_Valid_Organization()
        {
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
            var result = await _organizationAppService.GetListAsync(new PagedResultRequestDto());
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
            result.Items.ShouldContain(o => o.Name == _testData.OrganizationVolosoftName && o.Id == _testData.OrganizationVolosoftId);
            result.Items.ShouldContain(o => o.Name == _testData.OrganizationDotnetEuropeName && o.Id == _testData.OrganizationDotnetEuropeId);
        }
    }
}
