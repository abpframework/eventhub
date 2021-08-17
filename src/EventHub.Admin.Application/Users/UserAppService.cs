using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events.Registrations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace EventHub.Admin.Users
{
    public class UserAppService : EventHubAdminAppService, IUserAppService
    {
        private readonly IRepository<IdentityUser, Guid> _identityUserRepository;
        private readonly IRepository<EventRegistration, Guid> _eventRegistrationRepository;

        public UserAppService(IRepository<IdentityUser, Guid> identityUserRepository, IRepository<EventRegistration, Guid> eventRegistrationRepository)
        {
            _identityUserRepository = identityUserRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
        }

        public async Task<PagedResultDto<UserDto>> GetUnregisteredUsersOfEventAsync(GetUnregisteredUserEventInput input)
        {
            var identityUserQueryable = await _identityUserRepository.GetQueryableAsync();

            var eventRegistrationQuery = (await _eventRegistrationRepository.GetQueryableAsync())
                .Where(x => x.EventId == input.EventId)
                .Select(x => x.UserId);

            var userIds = await AsyncExecuter.ToListAsync(eventRegistrationQuery);

            var query = identityUserQueryable
                .Where(x => !userIds.Contains(x.Id))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Username), user => user.UserName.ToLower().Contains(input.Username.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name), user => user.Name.ToLower().Contains(input.Name.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Surname), user => user.Surname.ToLower().Contains(input.Surname.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Email), user => user.Email.ToLower().Contains(input.Email.ToLower()));

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.PageBy(input);

            var users = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<UserDto>(totalCount, ObjectMapper.Map<List<IdentityUser>, List<UserDto>>(users));
        }
    }
}
