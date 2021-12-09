using System.Threading.Tasks;
using EventHub.Members;
using EventHub.Users;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Controllers.Users
{
    [RemoteService(Name = EventHubRemoteServiceConsts.RemoteServiceName)]
    [Area("eventhubm")]
    [ControllerName("User")]
    [Route("/api/eventhub/user")]
    public class UserController : EventHubController, IUserAppService
    {
        private readonly IUserAppService _userAppService;

        public UserController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [HttpGet]
        [Route("{username}")]
        public async Task<UserDto> FindByUserNameAsync(string username)
        {
            return await _userAppService.FindByUserNameAsync(username);
        }
    }
}
