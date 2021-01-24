using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationAppServiceTests : EventHubApplicationTestBase
    {
        private readonly IEventRegistrationAppService _eventRegistrationAppService;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly EventHubTestData _testData;
        private readonly ICurrentUser _currentUser;

        public EventRegistrationAppServiceTests()
        {
            _eventRegistrationAppService = GetRequiredService<IEventRegistrationAppService>();
            _eventRegistrationRepository = GetRequiredService<IRepository<EventRegistration, Guid>>();
            _testData = GetRequiredService<EventHubTestData>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

        [Fact]
        public async Task Should_Register_To_An_Event()
        {
            await _eventRegistrationAppService.RegisterAsync(
                _testData.AbpMicroservicesFutureEventId
            );

            (await GetRegistrationOrNull(
                _testData.AbpMicroservicesFutureEventId,
                _currentUser.GetId()
            )).ShouldNotBeNull();
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
                        _currentUser.GetId()
                    )
                );
            });

            await _eventRegistrationAppService.UnregisterAsync(
                _testData.AbpMicroservicesFutureEventId
            );

            (await GetRegistrationOrNull(
                _testData.AbpMicroservicesFutureEventId,
                _currentUser.GetId())
            ).ShouldBeNull();
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
