using System;
using System.Threading.Tasks;
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
            await _eventAppService.CreateAsync(
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
        }
    }
}
