using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Organizations.Memberships
{
    public interface IOrganizationMembershipRepository : IRepository<OrganizationMembership, Guid>
    {
        Task<List<OrganizationMemberWithDetails>> GetMemberListAsync(
            Guid? organizationId,
            Guid? userId,
            int skipCount, 
            int maxResultCount, 
            CancellationToken cancellationToken = default);
        
        Task<int> GetCountAsync(
            Guid? organizationId,
            Guid? userId,
            CancellationToken cancellationToken = default);
    }
}