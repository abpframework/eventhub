using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EventHub.Events.Registrations
{
    public interface IEventRegistrationAppService : IApplicationService
    {
        Task RegisterAsync(Guid id);

        Task UnregisterAsync(Guid id);
    }
}
