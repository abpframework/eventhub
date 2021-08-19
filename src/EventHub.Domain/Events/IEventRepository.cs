using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Events
{
    public interface IEventRepository : IRepository<Event, Guid>
    {
        Task<int> GetCountAsync(
            string title = null,
            string organizationDisplayName = null,
            int? minAttendeeCount = null,
            int? maxAttendeeCount = null,
            DateTime? minStartTime = null,
            DateTime? maxStartTime = null,
            CancellationToken cancellationToken = default
        );

        Task<List<EventWithDetails>> GetListAsync(
            string sorting = null,
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            string title = null,
            string organizationDisplayName = null,
            int? minAttendeeCount = null,
            int? maxAttendeeCount = null,
            DateTime? minStartTime = null,
            DateTime? maxStartTime = null,
            CancellationToken cancellationToken = default
        );
    }
}
