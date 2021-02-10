using System;

namespace EventHub.Events
{
    public class UpdateEventTimingDto
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}