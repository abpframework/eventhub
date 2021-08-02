using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHub.Organizations;
using EventHub.Organizations.Memberships;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace EventHub.EntityFrameworkCore.Organizations.Memberships
{
    public class OrganizationMembershipRepository : EfCoreRepository<EventHubDbContext, OrganizationMembership, Guid>,
        IOrganizationMembershipRepository
    {
        public OrganizationMembershipRepository(IDbContextProvider<EventHubDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<OrganizationMemberWithDetails>> GetMemberListAsync(
            Guid? organizationId,
            Guid? userId,
            int skipCount,
            int maxResultCount,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            var organizationMembershipsDbSet = dbContext.Set<OrganizationMembership>();
            var identityUserDbSet = dbContext.Set<IdentityUser>();
            var organizationDbSet = dbContext.Set<Organization>();

            var query = organizationMembershipsDbSet
                .Join(organizationDbSet, t => t.OrganizationId, organization => organization.Id,
                    (organizationMember, organization) => new {organizationMember, organization})
                .Join(identityUserDbSet, organizationWithMember => organizationWithMember.organizationMember.UserId,
                    user => user.Id, (organizationWithMember, user) => new {organizationWithMember, user})
                .WhereIf(userId.HasValue, t => t.user.Id == userId)
                .WhereIf(organizationId.HasValue,
                    t => t.organizationWithMember.organizationMember.OrganizationId == organizationId)
                .OrderByDescending(o => o.organizationWithMember.organizationMember.CreationTime)
                .Select(t => new OrganizationMemberWithDetails
                {
                    OrganizationName = t.organizationWithMember.organization.Name,
                    UserName = t.user.UserName,
                    Email = t.user.Email,
                    Name = t.user.Name,
                    Surname = t.user.Surname
                });

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