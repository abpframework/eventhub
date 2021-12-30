using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace EventHub.EntityFrameworkCore.Events.Registrations
{
    public class EventRegistrationRepository : EfCoreRepository<EventHubDbContext, EventRegistration, Guid>, IEventRegistrationRepository
    {
        public EventRegistrationRepository(IDbContextProvider<EventHubDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<int> GetCountAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var userQueryable = (await GetDbContextAsync()).Set<IdentityUser>().AsQueryable();

            var query = from eventRegistration in (await GetQueryableAsync())
                        join user in userQueryable on eventRegistration.UserId equals user.Id
                        where eventRegistration.EventId == eventId
                        select user;

            return await query.CountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<EventRegistrationWithDetails>> GetListAsync(
            Guid eventId,
            string sorting = null,
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            CancellationToken cancellationToken = default
        )
        {
            var userQueryable = (await GetDbContextAsync()).Set<IdentityUser>().AsQueryable();

            var query = (from eventRegistration in (await GetQueryableAsync())
                join user in userQueryable on eventRegistration.UserId equals user.Id
                where eventRegistration.EventId == eventId
                select new EventRegistrationWithDetails
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    Email = user.Email,
                    CreationTime = user.CreationTime
                })
                .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(EventRegistrationWithDetails.CreationTime) : sorting)
                .PageBy(skipCount, maxResultCount);

            return await query.ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<bool> ExistsAsync(Guid eventId, Guid userId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.AnyAsync(x => x.EventId == eventId && x.UserId == userId);
        }
    }
}
