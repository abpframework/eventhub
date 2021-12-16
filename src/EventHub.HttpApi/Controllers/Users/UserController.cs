using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Members;
using EventHub.Users;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace EventHub.Controllers.Users
{
    [RemoteService(Name = EventHubRemoteServiceConsts.RemoteServiceName)]
    [Area("eventhub")]
    [ControllerName("User")]
    [Route("/api/eventhub/users")]
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

        [HttpGet]
        [Route("contains/{username}")]
        public async Task<List<UserInListDto>> GetListByUserName(string username)
        {
            username = username.Trim().ToLower();
            return await _userAppService.GetListByUserName(username);
        }
    }
}
