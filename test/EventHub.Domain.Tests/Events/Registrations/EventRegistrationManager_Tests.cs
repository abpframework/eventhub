using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationManager_Tests : EventHubDomainTestBase
    {
        private readonly EventRegistrationManager _eventRegistrationManager;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly EventHubTestData _testData;

        public EventRegistrationManager_Tests()
        {
            _eventRegistrationManager = GetRequiredService<EventRegistrationManager>();
            _eventRegistrationRepository = GetRequiredService<IRepository<EventRegistration, Guid>>();
            _testData = GetRequiredService<EventHubTestData>();
        }

        [Fact]
        public async Task Should_Register_To_An_Event()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var user = await GetUserAsync(_testData.UserAdminId);
                var @event = await GetEventAsync(_testData.AbpMicroservicesFutureEventId);
                await _eventRegistrationManager.RegisterAsync(@event, user);
            });

            (await GetRegistrationOrNull(_testData.AbpMicroservicesFutureEventId, _testData.UserAdminId))
                .ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Not_Register_To_An_Event_In_The_Past()
        {
            var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    var user = await GetUserAsync(_testData.UserAdminId);
                    var @event = await GetEventAsync(_testData.AbpBlazorPastEventId);
                    await _eventRegistrationManager.RegisterAsync(@event, user);
                });
            });

            exception.Code.ShouldBe(EventHubErrorCodes.CantRegisterOrUnregisterForAPastEvent);
        }

        [Fact]
        public async Task Should_Not_Register_Multiple_Times()
        {
            // Register
            await WithUnitOfWorkAsync(async () =>
            {
                var user = await GetUserAsync(_testData.UserAdminId);
                var @event = await GetEventAsync(_testData.AbpMicroservicesFutureEventId);
                await _eventRegistrationManager.RegisterAsync(@event, user);
            });

            // Register again
            await WithUnitOfWorkAsync(async () =>
            {
                var user = await GetUserAsync(_testData.UserAdminId);
                var @event = await GetEventAsync(_testData.AbpMicroservicesFutureEventId);
                await _eventRegistrationManager.RegisterAsync(@event, user);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var registrationCount = await _eventRegistrationRepository.CountAsync(
                    x => x.EventId == _testData.AbpMicroservicesFutureEventId && x.UserId == _testData.UserAdminId
                );
                registrationCount.ShouldBe(1);
            });
        }

        [Fact]
        public async Task Should_Unregister_From_An_Event()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await _eventRegistrationRepository.InsertAsync(
                    new EventRegistration(
                        Guid.NewGuid(),
                        _testData.AbpMicroservicesFutureEventId,
                        _testData.UserAdminId
                    )
                );
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var user = await GetUserAsync(_testData.UserAdminId);
                var @event = await GetEventAsync(_testData.AbpMicroservicesFutureEventId);
                await _eventRegistrationManager.UnregisterAsync(@event, user);
            });

            (await GetRegistrationOrNull(_testData.AbpMicroservicesFutureEventId, _testData.UserAdminId))
                .ShouldBeNull();
        }

        private async Task<EventRegistration> GetRegistrationOrNull(Guid eventId, Guid userId)
        {
            return await WithUnitOfWorkAsync(async () =>
            {
                return await _eventRegistrationRepository.FirstOrDefaultAsync(
                    x => x.EventId == eventId && x.UserId == userId
                );
            });
        }
    }
}
