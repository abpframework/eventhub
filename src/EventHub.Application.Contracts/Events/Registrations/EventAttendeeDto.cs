using System;

namespace EventHub.Events.Registrations
{
    public class EventAttendeeDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}
