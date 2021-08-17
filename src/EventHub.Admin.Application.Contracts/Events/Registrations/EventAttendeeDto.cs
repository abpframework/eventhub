using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Events.Registrations
{
    public class EventAttendeeDto : EntityDto<Guid>
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}
