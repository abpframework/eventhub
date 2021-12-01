using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Admin.Events.Registrations
{
    public interface IEventRegistrationAppService : IApplicationService
    {
        Task<PagedResultDto<EventAttendeeDto>> GetAttendeesAsync(GetEventRegistrationListInput input);

        Task UnRegisterAttendeeAsync(Guid eventId, Guid attendeeId);

        Task RegisterUsersAsync(Guid eventId, List<Guid> userIds);

        Task<List<Guid>> GetAllAttendeeIdsAsync(Guid eventId);
    }
}
