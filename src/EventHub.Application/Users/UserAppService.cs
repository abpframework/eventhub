using System;
using System.Threading.Tasks;
using EventHub.Members;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Users
{
    public class UserAppService : EventHubAppService, IUserAppService
    {
        private readonly IRepository<AppUser, Guid> _userRepository;
        
        public UserAppService(IRepository<AppUser, Guid> userRepository)
        {
            _userRepository = userRepository;
        }
        
        public async Task<UserDto> FindByUserNameAsync(string username)
        {
            var user = await _userRepository.FindAsync(x => x.UserName == username);
            
            return ObjectMapper.Map<AppUser, UserDto>(user);
        }
    }
}