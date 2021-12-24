using System;
using Volo.Abp.Domain.Entities;

namespace EventHub.Events
{
    public class Speaker : Entity
    {
        public Guid SessionId { get; private set; }
        
        public Guid UserId { get; private set; }

        private Speaker()
        {
            
        }
        
        public Speaker(Guid sessionId, Guid userId)
        {
            SessionId = sessionId;
            UserId = userId;
        }

        public override object[] GetKeys()
        {
            return new object[] {SessionId, UserId};
        }
    }
}
