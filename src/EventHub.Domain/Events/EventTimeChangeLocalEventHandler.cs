using System;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using IdentityServer4.Validation;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.Linq;

namespace EventHub.Events
{
    public class EventTimeChangeLocalEventHandler : ILocalEventHandler<EventTimeChangingEventData>, ITransientDependency
    {
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;
        private readonly IAsyncQueryableExecuter _asyncExecuter;

        public EventTimeChangeLocalEventHandler(
            IRepository<EventRegistration, Guid> eventRegistrationRepository,
            IAsyncQueryableExecuter asyncExecuter)
        {
            _eventRegistrationRepository = eventRegistrationRepository;
            _asyncExecuter = asyncExecuter;
        }
        
        public async Task HandleEventAsync(EventTimeChangingEventData eventData)
        {
            await UpdateEventRegistrationForTimingChangeAsync(@eventData.Event);
        }
        
        private async Task UpdateEventRegistrationForTimingChangeAsync(Event @event)
        {
            var eventRegistrationQueryable = await _eventRegistrationRepository.GetQueryableAsync();
            var query = eventRegistrationQueryable.Where(x => x.EventId == @event.Id);
            var eventRegistrations = await _asyncExecuter.ToListAsync(query);

            foreach (var eventRegistration in eventRegistrations)
            {
                if (eventRegistration.IsTimingChangeEmailSent)
                {
                    eventRegistration.IsTimingChangeEmailSent = false;
                }
            }

            await _eventRegistrationRepository.UpdateManyAsync(eventRegistrations, true);
        }
    }
}