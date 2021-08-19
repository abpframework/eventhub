using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Admin.Permissions;
using EventHub.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;

namespace EventHub.Admin.Users
{
    [Authorize(EventHubPermissions.Users.Default)]
    public class UserAppService : EventHubAdminAppService, IUserAppService
    {
        private readonly IUserRepository _userRepository;

        public UserAppService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<UserDto>> GetListAsync(GetUserListInput input)
        {
            var totalCount = await _userRepository.GetCountAsync(input.Username);
            var items = await _userRepository.GetListAsync(input.Sorting, input.SkipCount, input.MaxResultCount, input.Username);
            var users = ObjectMapper.Map<List<IdentityUser>, List<UserDto>>(items);

            return new PagedResultDto<UserDto>(totalCount, users);
        }
    }
}
