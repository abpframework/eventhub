using System;
using System.Threading.Tasks;
using EventHub.Users;
using Volo.Abp.Domain.Repositories;

namespace EventHub.Members
{
    public class MemberAppService : EventHubAppService, IMemberAppService
    {
        private readonly IRepository<AppUser, Guid> _userRepository;
        
        public MemberAppService(IRepository<AppUser, Guid> userRepository)
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