using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace EventHub.Admin.Users
{
    public class UserDto : EntityDto<Guid>, IHasCreationTime
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
