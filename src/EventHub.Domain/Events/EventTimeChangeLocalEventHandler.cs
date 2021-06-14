using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;

namespace EventHub.Events
{
    public class EventTimeChangeLocalEventHandler : ILocalEventHandler<EventTimeChangingEventData>, ITransientDependency
    {
        private readonly IRepository<Event, Guid> _eventRepository;

        public EventTimeChangeLocalEventHandler(IRepository<Event, Guid> eventRepository)
        {
            _eventRepository = eventRepository;
        }
        
        public async Task HandleEventAsync(EventTimeChangingEventData eventData)
        {
            await UpdateEventForTimingChangeAsync(eventData.Event);
        }
        
        private async Task UpdateEventForTimingChangeAsync(Event @event)
        {
            @event.IsTimingChangeEmailSent = false;

            await _eventRepository.UpdateAsync(@event, true);
        }
    }
}