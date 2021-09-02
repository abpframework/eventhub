using System;

namespace EventHub.Events
{
    public class EventWithDetails
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string OrganizationDisplayName { get; set; }

        public int AttendeeCount { get; set; }

        public DateTime StartTime { get; set; }
    }
}
