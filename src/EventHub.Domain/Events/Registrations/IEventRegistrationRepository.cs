using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Events.Registrations
{
    public interface IEventRegistrationRepository : IRepository<EventRegistration, Guid>
    {
        Task<int> GetCountAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<List<EventRegistrationWithDetails>> GetListAsync(
            Guid eventId, 
            string sorting = null, 
            int skipCount = 0, 
            int maxResultCount = int.MaxValue, 
            CancellationToken cancellationToken = default
        );

        Task<bool> ExistsAsync(Guid eventId, Guid userId);
    }
}
