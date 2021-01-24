using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Timing;
using Xunit;

namespace EventHub.Events
{
    public class EventAppServiceTests : EventHubApplicationTestBase
    {
        private readonly IEventAppService _eventAppService;
        private readonly EventHubTestData _testData;

        public EventAppServiceTests()
        {
            _eventAppService = GetRequiredService<IEventAppService>();
            _testData = GetRequiredService<EventHubTestData>();
        }

        [Fact]
        public async Task Should_Create_A_New_Valid_Event()
        {
            var eventDto = await _eventAppService.CreateAsync(
                new CreateEventDto
                {
                    OrganizationId = _testData.OrganizationVolosoftId,
                    Title = "Introduction to the ABP Framework",
                    Description = "In this event, we will introduce the ABP Framework and explore the fundamental features.",
                    StartTime = DateTime.Now.AddDays(1),
                    EndTime = DateTime.Now.AddDays(1).AddHours(3),
                    IsOnline = true
                }
            );

            eventDto.OrganizationId.ShouldBe(_testData.OrganizationVolosoftId);
            eventDto.Title.ShouldBe("Introduction to the ABP Framework");
            eventDto.Description.ShouldBe("In this event, we will introduce the ABP Framework and explore the fundamental features.");
            eventDto.IsOnline.ShouldBeTrue();
            eventDto.UrlCode.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Should_Not_Create_Event_For_Not_Authorized_Organization()
        {
            var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _eventAppService.CreateAsync(
                    new CreateEventDto
                    {
                        OrganizationId = _testData.OrganizationDotnetEuropeId,
                        Title = "Introduction to the ABP Framework",
                        Description = "In this event, we will introduce the ABP Framework and explore the fundamental features.",
                        StartTime = DateTime.Now.AddDays(1),
                        EndTime = DateTime.Now.AddDays(1).AddHours(3),
                        IsOnline = true
                    }
                );
            });

            exception.Code.ShouldBe(EventHubErrorCodes.NotAuthorizedToCreateEventInThisOrganization);
        }

        [Fact]
        public async Task Should_Get_Upcoming_Events()
        {
            var now = GetRequiredService<IClock>().Now;

            var result = await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    MinDate = now
                }
            );

            result.TotalCount.ShouldBeGreaterThanOrEqualTo(1);
            result.Items.ShouldContain(x =>
                x.Id == _testData.AbpMicroservicesFutureEventId &&
                x.Title == _testData.AbpMicroservicesFutureEventTitle &&
                x.UrlCode == _testData.AbpMicroservicesFutureEventUrlCode
            );
            result.Items.ShouldAllBe(x => x.EndTime >= now);
        }

        [Fact]
        public async Task Should_Get_Past_Events()
        {
            var now = GetRequiredService<IClock>().Now;

            var result = await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    MaxDate = now
                }
            );

            result.TotalCount.ShouldBeGreaterThanOrEqualTo(1);
            result.Items.ShouldContain(x =>
                x.Id == _testData.AbpBlazorPastEventId &&
                x.Title == _testData.AbpBlazorPastEventTitle &&
                x.UrlCode == _testData.AbpBlazorPastEventUrlCode
            );
            result.Items.ShouldAllBe(x => x.EndTime <= now);
        }

        [Fact]
        public async Task Should_Get_By_UrlCode()
        {
            var eventDetailDto = await _eventAppService.GetByUrlCodeAsync(_testData.AbpBlazorPastEventUrlCode);

            eventDetailDto.Id.ShouldBe(_testData.AbpBlazorPastEventId);
            eventDetailDto.Title.ShouldBe(_testData.AbpBlazorPastEventTitle);
            eventDetailDto.UrlCode.ShouldBe(_testData.AbpBlazorPastEventUrlCode);
        }
    }
}
