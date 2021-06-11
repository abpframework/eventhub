using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

namespace EventHub.Members
{
    public class UserDto : EntityDto<Guid>, IHasCreationTime
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
        
        public string Email { get; set; }
        
        public DateTime CreationTime { get; set; }
    }
}