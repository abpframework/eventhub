using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace EventHub.Events.Registrations
{
    public class EventRegistration : CreationAuditedAggregateRoot<Guid>
    {
        public Guid EventId { get; private set; }

        public Guid UserId { get; private set; }
        
        private EventRegistration()
        {

        }

        internal EventRegistration(
            Guid id,
            Guid eventId,
            Guid userId)
            : base(id)
        {
            EventId = eventId;
            UserId = userId;
        }
    }
}
