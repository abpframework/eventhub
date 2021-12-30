using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Admin.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<PagedResultDto<UserDto>> GetListAsync(GetUserListInput input);
    }
}
