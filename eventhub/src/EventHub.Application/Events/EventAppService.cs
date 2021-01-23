using System.Threading.Tasks;

namespace EventHub.Events
{
    public class EventAppService : EventHubAppService, IEventAppService
    {
        public async Task CreateAsync(CreateEventDto input)
        {

        }
    }
}
