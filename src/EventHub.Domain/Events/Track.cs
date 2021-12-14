using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace EventHub.Events
{
    public class Track : Entity<Guid>
    {
        public Guid EventId { get; private set; }
        
        public string Name { get; private set; }
        
        public ICollection<Session> Sessions { get; private set; }

        private Track()
        {
            
        }
        
        internal Track(
            Guid id,
            Guid eventId,
            string name)
            : base(id)
        {
            EventId = eventId;
            SetName(name);
            Sessions = new Collection<Session>();
        }
        
        public Track SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), TrackConsts.MaxNameLength);
            return this;
        }

        internal Track AddSession(
            Guid sessionId,
            string title,
            DateTime startTime,
            DateTime endTime, 
            string description,
            string language)
        {
            if (startTime > endTime)
            {
                throw new BusinessException(EventHubErrorCodes.EndTimeCantBeEarlierThanStartTime);
            }

            foreach (var session in Sessions)
            {
                if (startTime.IsBetween(session.StartTime, session.EndTime) ||
                    endTime.IsBetween(session.StartTime, session.EndTime))
                {
                    throw new BusinessException(EventHubErrorCodes.SessionTimeConflictsWithAnExistingSession);
                }
            }
            
            Sessions.Add(new Session(sessionId, Id, title, startTime, endTime, description, language));
            
            return this;
        }
    }
}
