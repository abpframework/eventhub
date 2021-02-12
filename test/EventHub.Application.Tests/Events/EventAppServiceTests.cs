using System;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using Xunit;

namespace EventHub.Events
{
    public class EventAppServiceTests : EventHubApplicationTestBase
    {
        private readonly IEventAppService _eventAppService;
        private readonly IEventRegistrationAppService _eventRegistrationAppService;
        private readonly EventHubTestData _testData;
        private readonly IRepository<Event, Guid> _eventRepository;

        public EventAppServiceTests()
        {
            _eventAppService = GetRequiredService<IEventAppService>();
            _eventRegistrationAppService = GetRequiredService<IEventRegistrationAppService>();
            _testData = GetRequiredService<EventHubTestData>();
            _eventRepository = GetRequiredService<IRepository<Event, Guid>>();
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
                    IsOnline = true,
                    Capacity = 2,
                    Language = "en"
                }
            );

            eventDto.OrganizationId.ShouldBe(_testData.OrganizationVolosoftId);
            eventDto.Title.ShouldBe("Introduction to the ABP Framework");
            eventDto.Description.ShouldBe("In this event, we will introduce the ABP Framework and explore the fundamental features.");
            eventDto.IsOnline.ShouldBeTrue();
            eventDto.UrlCode.ShouldNotBeNullOrWhiteSpace();
            eventDto.Capacity.ShouldBe(2);
            eventDto.Language.ShouldBe("en");
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

        [Fact]
        public async Task Should_Get_Location()
        {
            var eventDto = await _eventAppService.CreateAsync(
                new CreateEventDto
                {
                    OrganizationId = _testData.OrganizationVolosoftId,
                    Title = "Introduction to the ABP Framework",
                    Description = "In this event, we will introduce the ABP Framework and explore the fundamental features.",
                    StartTime = DateTime.Now.AddDays(2),
                    EndTime = DateTime.Now.AddDays(2).AddHours(3),
                    IsOnline = true,
                    Capacity = 2,
                    Language = "en",
                    City = "Istanbul",
                    OnlineLink = "http://abp.io"
                }
            );

            await _eventRegistrationAppService.RegisterAsync(eventDto.Id);
            
            var result = await _eventAppService.GetLocationAsync(eventDto.Id);

            result.ShouldNotBeNull();
            result.IsOnline.ShouldBeTrue();
            result.OnlineLink.ShouldBe("http://abp.io");
            result.City.ShouldBeNull();
        }
        
        [Fact]
        public async Task Should_Get_All_Countries()
        {
            var result = await _eventAppService.GetCountriesLookupAsync();

            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(1);
        }

        [Fact]
        public async Task Should_Update_The_Event()
        {
            await _eventAppService.UpdateAsync(
                _testData.AbpMicroservicesFutureEventId,
                new UpdateEventDto
                {
                    Title = "Updated_Microservices_Event_Title",
                    Description = "Updated_Microservices_Event_Description-Updated_Microservices_Event_Description-Updated_Blazor_Microservices_Description",
                    IsOnline = true,
                    Capacity = 100,
                    City = null,
                    CountryId = null,
                    Language = "en",
                    OnlineLink = null
                });

            var updatedEvent = await _eventRepository.GetAsync(_testData.AbpMicroservicesFutureEventId);
            
            updatedEvent.ShouldNotBeNull();
            updatedEvent.Title.ShouldBe("Updated_Microservices_Event_Title");
            updatedEvent.Description.ShouldBe("Updated_Microservices_Event_Description-Updated_Microservices_Event_Description-Updated_Blazor_Microservices_Description");
            updatedEvent.IsOnline.ShouldBeTrue();
        }
    }
}
