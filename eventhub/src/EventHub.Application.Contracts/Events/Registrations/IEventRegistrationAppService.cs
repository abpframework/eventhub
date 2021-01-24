using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EventHub.Events.Registrations
{
    public interface IEventRegistrationAppService : IApplicationService
    {
        Task RegisterAsync(Guid eventId);

        Task UnregisterAsync(Guid eventId);

        Task<bool> IsRegisteredAsync(Guid eventId);
    }
}
