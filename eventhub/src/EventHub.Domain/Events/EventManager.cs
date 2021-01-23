using System;
using System.Threading.Tasks;
using EventHub.Organizations;
using Volo.Abp.Domain.Services;

namespace EventHub.Events
{
    public class EventManager : DomainService
    {
        public async Task<Event> CreateAsync(
            Organization organization,
            string title,
            DateTime startTime,
            DateTime endTime,
            string description)
        {
            return new Event(
                GuidGenerator.Create(),
                organization.Id,
                title,
                startTime,
                endTime,
                description
            );
        }
    }
}
