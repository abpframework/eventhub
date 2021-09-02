using System;
using Volo.Abp.Auditing;

namespace EventHub.Events.Registrations
{
    public class EventRegistrationWithDetails : IHasCreationTime
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
