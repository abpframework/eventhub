using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using EventHub.Users;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace EventHub.EntityFrameworkCore.Users
{
    public class UserRepository : EfCoreRepository<EventHubDbContext, IdentityUser, Guid>, IUserRepository
    {
        public UserRepository(IDbContextProvider<EventHubDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<int> GetCountAsync(string username = null, CancellationToken cancellationToken = default)
        {
            return await (await GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(username), user => user.UserName.ToLower().Contains(username.ToLower()))
                .CountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<IdentityUser>> GetListAsync(
            string sorting = null, 
            int skipCount = 0, 
            int maxResultCount = int.MaxValue, 
            string username = null, 
            CancellationToken cancellationToken = default
        )
        {
            return await (await GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(username), user => user.UserName.ToLower().Contains(username.ToLower()))
                .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(IdentityUser.CreationTime) : sorting)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
    }
}
