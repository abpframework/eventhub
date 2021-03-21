using System;

namespace EventHub.Events
{
    public class EventTimeChangingEventData
    {
        public Event Event { get; }
        public DateTime OldStartTime { get; }
        public DateTime OldEndTime { get; }

        public EventTimeChangingEventData(Event @event, DateTime oldStartTime, DateTime oldEndTime)
        {
            Event = @event;
            OldStartTime = oldStartTime;
            OldEndTime = oldEndTime;
        }
    }
}