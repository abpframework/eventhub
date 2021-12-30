using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using Volo.Abp.Users;
using Xunit;

namespace EventHub.Events
{
    public class EventAppServiceTests : EventHubApplicationTestBase
    {
        private readonly IEventAppService _eventAppService;
        private readonly IEventRegistrationAppService _eventRegistrationAppService;
        private readonly EventHubTestData _testData;
        private readonly IRepository<Event, Guid> _eventRepository;
        private ICurrentUser _currentUser;

        public EventAppServiceTests()
        {
            _eventAppService = GetRequiredService<IEventAppService>();
            _eventRegistrationAppService = GetRequiredService<IEventRegistrationAppService>();
            _testData = GetRequiredService<EventHubTestData>();
            _eventRepository = GetRequiredService<IRepository<Event, Guid>>();
        }
        
        protected override void AfterAddApplication(IServiceCollection services)
        {
            _currentUser = Substitute.For<ICurrentUser>();
            services.AddSingleton(_currentUser);
        }
        
        [Fact]
        public async Task Should_True_To_A_Owned_Event()
        {
            Login(_testData.UserAdminId);

            var result = await _eventAppService.IsEventOwnerAsync(
                _testData.AbpBlazorPastEventId
            );

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_False_To_Not_Owned_Event()
        {
            Login(_testData.UserJohnId);
            
            var result = await _eventAppService.IsEventOwnerAsync(
                _testData.AbpBlazorPastEventId
            );

            result.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_Create_A_New_Valid_Event()
        {
            Login(_testData.UserAdminId);

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
            Login(_testData.UserAdminId);

            var exception = await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
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
            Login(_testData.UserAdminId);

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
            Login(_testData.UserAdminId);

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
        
        [Fact]
        public async Task Should_Add_The_Track()
        {
            Login(_testData.UserAdminId);

            var eventDetailDto = await _eventAppService.GetByUrlCodeAsync(_testData.AbpMicroservicesFutureEventUrlCode);

            await _eventAppService.AddTrackAsync(eventDetailDto.Id, new AddTrackDto
            {
                Name = "Track-1"
            });

            var updatedEvent = await _eventAppService.GetByUrlCodeAsync(_testData.AbpMicroservicesFutureEventUrlCode);
            
            updatedEvent.ShouldNotBeNull();
            updatedEvent.Tracks.ShouldContain(x => x.Name == "Track-1");
        }
        
        [Fact]
        public async Task Should_Add_The_Session()
        {
            Login(_testData.UserAdminId);

            var eventDetailDto = await _eventAppService.GetByUrlCodeAsync(_testData.AbpMicroservicesFutureEventUrlCode);

            await _eventAppService.AddTrackAsync(eventDetailDto.Id, new AddTrackDto
            {
                Name = "Track-1"
            });

            var updatedEvent = await _eventAppService.GetByUrlCodeAsync(_testData.AbpMicroservicesFutureEventUrlCode);

            var speakerUserNames = new List<string>();
            speakerUserNames.Add(_testData.UserAdminUserName);
            speakerUserNames.Add(_testData.UserJohnUserName);
            await _eventAppService.AddSessionAsync(eventDetailDto.Id, updatedEvent.Tracks.First().Id, new AddSessionDto
            {
                Title = "Session-1 Title",
                Description = "Session-1 Description".PadLeft(50, 't'),
                StartTime = updatedEvent.StartTime.AddSeconds(1),
                EndTime = updatedEvent.StartTime.AddSeconds(50),
                Language = "tr",
                SpeakerUserNames = speakerUserNames
            });
            
            updatedEvent = await _eventAppService.GetByUrlCodeAsync(_testData.AbpMicroservicesFutureEventUrlCode);
            
            updatedEvent.ShouldNotBeNull();
            updatedEvent.Tracks.ShouldContain(x => x.Name == "Track-1");
            var track = updatedEvent.Tracks.Single(x => x.Name == "Track-1");
           
            
            track.Sessions.ShouldContain(x => x.Title == "Session-1 Title");
            track.Sessions.ShouldContain(x => x.StartTime == updatedEvent.StartTime.AddSeconds(1));
            track.Sessions.ShouldContain(x => x.EndTime == updatedEvent.StartTime.AddSeconds(50));
            track.Sessions.ShouldContain(x => x.Language == "tr");
            
            var session = track.Sessions.Single(x => x.Title == "Session-1 Title");
            session.Speakers.ShouldContain(x => x.UserId == _testData.UserAdminId);
            session.Speakers.ShouldContain(x => x.UserId == _testData.UserJohnId);
        }
        
        [Fact]
        public async Task Should_Publish_Event()
        {
            Login(_testData.UserAdminId);

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

            Login(_testData.UserJohnId);
            
            var exception = await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await _eventAppService.GetByUrlCodeAsync(eventDto.UrlCode);
            });
            
            exception.Code.ShouldBe(EventHubErrorCodes.NotAuthorizedToUpdateEvent);
            
            Login(_testData.UserAdminId);
            
            await _eventAppService.PublishAsync(eventDto.Id);
            
            Login(_testData.UserJohnId);
            
            var @event = await _eventAppService.GetByUrlCodeAsync(eventDto.UrlCode);
            @event.ShouldNotBeNull();
            @event.Title.ShouldBe("Introduction to the ABP Framework");
        }

        private void Login(Guid userId)
        {
            _currentUser.Id.Returns(userId);
            _currentUser.IsAuthenticated.Returns(true);
        }
    }
}
