using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace EventHub.Admin.Users
{
    public class UserAppService : EventHubAdminAppService, IUserAppService
    {
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

        public UserAppService(IRepository<IdentityUser, Guid> identityUserRepository)
        {
            _identityUserRepository = identityUserRepository;
        }

        public async Task<PagedResultDto<UserDto>> GetListAsync(GetUserListInput input)
        {
            var identityUserQueryable = await _identityUserRepository.GetQueryableAsync();

            var query = identityUserQueryable
                .WhereIf(!string.IsNullOrWhiteSpace(input.Username), user => user.UserName.ToLower().Contains(input.Username.ToLower()));

            var totalCount = await AsyncExecuter.CountAsync(query);

            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                query = query.OrderBy(input.Sorting);
            }

            query = query.PageBy(input);

            var users = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<UserDto>(totalCount, ObjectMapper.Map<List<IdentityUser>, List<UserDto>>(users));
        }
    }
}
