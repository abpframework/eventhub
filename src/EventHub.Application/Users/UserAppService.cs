using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Members;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Identity;

namespace EventHub.Users
{
    public class UserAppService : EventHubAppService, IUserAppService
    {
        private readonly IUserRepository _userRepository;
        
        public UserAppService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<UserDto> FindByUserNameAsync(string username)
        {
            var user = await _userRepository.FindAsync(x => x.UserName == username);
            
            return ObjectMapper.Map<IdentityUser, UserDto>(user);
        }
        
        [Authorize]
        public async Task<List<UserInListDto>> GetListByUserName(string username)
        {
            var userQueryable = await _userRepository.GetQueryableAsync();

            var query = userQueryable
                .Where(u => u.UserName.ToLower().Contains(username))
                .Select(x => new UserWithoutDetails
                {
                    Id = x.Id,
                    UserName = x.UserName
                }).Take(10);

            return ObjectMapper.Map<List<UserWithoutDetails>, List<UserInListDto>>(await AsyncExecuter.ToListAsync(query));
        }
    }
}
