using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationRepository_Tests : EventHubDomainTestBase
    {
        private readonly IEventRegistrationRepository _repository;
        private readonly EventHubTestData _testData;

        public EventRegistrationRepository_Tests()
        {
            _repository = GetRequiredService<IEventRegistrationRepository>();
            _testData = GetRequiredService<EventHubTestData>();
        }

        [Fact]
        public async Task Exists_Should_Return_False_If_Not_Registered()
        {
            var exists = await _repository.ExistsAsync(
                _testData.AbpMicroservicesFutureEventId, 
                _testData.UserJohnId);
            exists.ShouldBeFalse();

            await _repository.InsertAsync(
                new EventRegistration(
                    Guid.NewGuid(),
                    _testData.AbpMicroservicesFutureEventId,
                    _testData.UserJohnId));
        }
        
        [Fact]
        public async Task Exists_Should_Return_True_If_Registered()
        {
            await _repository.InsertAsync(
                new EventRegistration(
                    Guid.NewGuid(),
                    _testData.AbpMicroservicesFutureEventId,
                    _testData.UserJohnId));
            
            var exists = await _repository.ExistsAsync(
                _testData.AbpMicroservicesFutureEventId, 
                _testData.UserJohnId);
            exists.ShouldBeTrue();
        }
        
        [Fact]
        public async Task Test_With_Uow()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var queryable = await _repository.GetQueryableAsync();
                var exists = await queryable.Where(
                    x => x.EventId == _testData.AbpMicroservicesFutureEventId &&
                         x.UserId == _testData.UserJohnId
                ).FirstOrDefaultAsync();
                exists.ShouldBeNull();
            });
        }
    }
}