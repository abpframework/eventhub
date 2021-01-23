using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Volo.Abp.Domain.Services;

namespace EventHub.Events
{
    public class EventManager : DomainService
    {
        public ValueTask<Event> CreateAsync(
            Organization organization,
            string title,
            DateTime startTime,
            DateTime endTime,
            string description)
        {
            return ValueTask.FromResult(
                new Event(
                    GuidGenerator.Create(),
                    organization.Id,
                    title,
                    startTime,
                    endTime,
                    description
                )
            );
        }
    }
}
