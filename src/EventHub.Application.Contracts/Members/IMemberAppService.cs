using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EventHub.Members
{
    public interface IMemberAppService : IApplicationService
    {
        Task<UserDto> FindByUserNameAsync(string username);
    }
}