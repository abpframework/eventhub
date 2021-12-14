using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Members;
using Volo.Abp.Application.Services;

namespace EventHub.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<UserDto> FindByUserNameAsync(string username);

        Task<List<UserInListDto>> GetListByUserName(string username);
    }
}
