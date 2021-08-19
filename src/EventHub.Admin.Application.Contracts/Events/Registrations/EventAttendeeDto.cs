using System;

namespace EventHub.Admin.Events.Registrations
{
    public class EventAttendeeDto
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}
