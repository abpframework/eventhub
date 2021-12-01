using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace EventHub.Users
{
    public interface IUserRepository : IRepository<IdentityUser, Guid>
    {
        Task<List<IdentityUser>> GetListAsync(
            string sorting = null,
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            string username = null,
            CancellationToken cancellationToken = default
        );

        Task<int> GetCountAsync(
            string username = null, 
            CancellationToken cancellationToken = default
        );
    }
}
