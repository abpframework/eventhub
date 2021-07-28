using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHub.Organizations.Memberships;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace EventHub.EntityFrameworkCore.Organizations.Memberships
{
    public class OrganizationMembershipRepository : EfCoreRepository<EventHubDbContext, OrganizationMembership, Guid>, IOrganizationMembershipRepository
    {
        public OrganizationMembershipRepository(IDbContextProvider<EventHubDbContext> dbContextProvider) : base(dbContextProvider)
        {
            
        }

        public async Task<List<IdentityUser>> GetMemberListAsync(
            Guid? organizationId, 
            Guid? userId,
            int skipCount, 
            int maxResultCount,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            var organizationMembershipsDbSet = dbContext.Set<OrganizationMembership>();
            var identityUserDbSet = dbContext.Set<IdentityUser>();

            var query = organizationMembershipsDbSet
                .Join(identityUserDbSet, organizationMember => organizationMember.UserId, user => user.Id,
                    (organizationMember, user) => new {organizationMember, user})
                .WhereIf(userId.HasValue, t => t.user.Id == userId)
                .WhereIf(organizationId.HasValue, t => t.organizationMember.OrganizationId == organizationId)
                .OrderByDescending(x => x.organizationMember.CreationTime)
                .Select(x => x.user);
            
            return await query
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<int> GetCountAsync(
            Guid? organizationId, 
            Guid? userId,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            var organizationMembershipsDbSet = dbContext.Set<OrganizationMembership>();
            var identityUserDbSet = dbContext.Set<IdentityUser>();

            return await organizationMembershipsDbSet
                .Join(identityUserDbSet, organizationMember => organizationMember.UserId, user => user.Id,
                    (organizationMember, user) => new {organizationMember, user})
                .WhereIf(userId.HasValue, t => t.user.Id == userId)
                .WhereIf(organizationId.HasValue, t => t.organizationMember.OrganizationId == organizationId)
                .CountAsync(GetCancellationToken(cancellationToken));
        }
    }
}