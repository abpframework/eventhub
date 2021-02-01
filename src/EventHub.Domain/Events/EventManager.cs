using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Volo.Abp.Domain.Services;

namespace EventHub.Events
{
    public class EventManager : DomainService
    {
        private readonly EventUrlCodeGenerator _eventUrlCodeGenerator;

        public EventManager(EventUrlCodeGenerator eventUrlCodeGenerator)
        {
            _eventUrlCodeGenerator = eventUrlCodeGenerator;
        }

        public async Task<Event> CreateAsync(
            Organization organization,
            string title,
            DateTime startTime,
            DateTime endTime,
            string description)
        {
            //TODO: Check capacity and throw business exception!

            return new Event(
                GuidGenerator.Create(),
                organization.Id,
                await _eventUrlCodeGenerator.GenerateAsync(),
                title,
                startTime,
                endTime,
                description
            );
        }
    }
}
