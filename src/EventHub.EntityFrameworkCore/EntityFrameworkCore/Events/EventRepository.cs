using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EventHub.EntityFrameworkCore.Events
{
    public class EventRepository : EfCoreRepository<EventHubDbContext, Event, Guid>, IEventRepository
    {
        public EventRepository(IDbContextProvider<EventHubDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<int> GetCountAsync(
            string title = null,
            string organizationDisplayName = null,
            int? minAttendeeCount = null,
            int? maxAttendeeCount = null,
            DateTime? minStartTime = null,
            DateTime? maxStartTime = null,
            CancellationToken cancellationToken = default
        )
        {
            var dbContext = await GetDbContextAsync();

            var eventQueryable = await GetQueryableAsync();
            var eventRegistrationQueryable = dbContext.Set<EventRegistration>().AsQueryable();
            var organizationQueryable = dbContext.Set<Organization>().AsQueryable();

            var query = (from @event in eventQueryable
                    join organization in organizationQueryable on @event.OrganizationId equals organization.Id
                    select new
                    {
                        Event = @event,
                        Organization = organization,
                        AttendeeCount = (from eventRegistration in eventRegistrationQueryable
                            where eventRegistration.EventId == @event.Id
                            select @eventRegistration).Count()
                    })
                .WhereIf(!string.IsNullOrWhiteSpace(title), x => x.Event.Title.ToLower().Contains(title.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(organizationDisplayName),
                    x => x.Organization.DisplayName.ToLower().Contains(organizationDisplayName.ToLower()))
                .WhereIf(minStartTime.HasValue, x => x.Event.StartTime >= minStartTime)
                .WhereIf(maxStartTime.HasValue, x => x.Event.StartTime <= maxStartTime)
                .WhereIf(minAttendeeCount.HasValue, x => x.AttendeeCount >= minAttendeeCount)
                .WhereIf(maxAttendeeCount.HasValue, x => x.AttendeeCount <= maxAttendeeCount);

            return await query.CountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<EventWithDetails>> GetListAsync(
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
        )
        {
            var dbContext = await GetDbContextAsync();

            var eventQueryable = await GetQueryableAsync();
            var eventRegistrationQueryable = dbContext.Set<EventRegistration>().AsQueryable();
            var organizationQueryable = dbContext.Set<Organization>().AsQueryable();

            var query = (from @event in eventQueryable
                    join organization in organizationQueryable on @event.OrganizationId equals organization.Id
                    select new EventWithDetails
                    {
                        Id = @event.Id,
                        Title = @event.Title,
                        StartTime = @event.StartTime,
                        OrganizationDisplayName = organization.DisplayName,
                        AttendeeCount = (from eventRegistration in eventRegistrationQueryable
                            where eventRegistration.EventId == @event.Id
                            select @eventRegistration).Count()
                    })
                .WhereIf(!string.IsNullOrWhiteSpace(title), x => x.Title.ToLower().Contains(title.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(organizationDisplayName),
                    x => x.OrganizationDisplayName.ToLower().Contains(organizationDisplayName.ToLower()))
                .WhereIf(minStartTime.HasValue, x => x.StartTime >= minStartTime)
                .WhereIf(maxStartTime.HasValue, x => x.StartTime <= maxStartTime)
                .WhereIf(minAttendeeCount.HasValue, x => x.AttendeeCount >= minAttendeeCount)
                .WhereIf(maxAttendeeCount.HasValue, x => x.AttendeeCount <= maxAttendeeCount)
                .OrderBy(string.IsNullOrWhiteSpace(sorting) ? EventConsts.DefaultSorting : sorting)
                .PageBy(skipCount, maxResultCount);

            return await query.ToListAsync(GetCancellationToken(cancellationToken));
        }

        public override async Task<IQueryable<Event>> WithDetailsAsync()
        {
            return (await GetQueryableAsync())
                .Include(e => e.Tracks)
                .ThenInclude(t => t.Sessions)
                .ThenInclude(s => s.Speakers);
        }
    }
}
