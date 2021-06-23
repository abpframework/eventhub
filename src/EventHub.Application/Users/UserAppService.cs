using System;
using System.Threading.Tasks;
using EventHub.Members;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace EventHub.Users
{
    public class UserAppService : EventHubAppService, IUserAppService
    {
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        
        public UserAppService(IRepository<IdentityUser, Guid> userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<UserDto> FindByUserNameAsync(string username)
        {
            var user = await _userRepository.FindAsync(x => x.UserName == username);
            
            return ObjectMapper.Map<IdentityUser, UserDto>(user);
        }
    }
}