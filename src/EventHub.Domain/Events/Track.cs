using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            string description,
            DateTime startTime,
            DateTime endTime,
            string language,
            ICollection<Guid> speakerUserIds)
        {
            if (Sessions.Any(s => s.Title == title))
            {
                throw new BusinessException(EventHubErrorCodes.SessionTitleAlreadyExist)
                    .WithData("Title", title);
            }
            
            if (startTime > endTime)
            {
                throw new BusinessException(EventHubErrorCodes.SessionEndTimeCantBeEarlierThanStartTime);
            }

            CheckIfValidSessionTime(startTime);
            CheckIfValidSessionTime(endTime);

            Sessions.Add(new Session(sessionId, Id, title, startTime, endTime, description, language, speakerUserIds));
            
            return this;
        }

        internal Track UpdateSession(
            Guid sessionId,
            string title,
            string description,
            DateTime startTime,
            DateTime endTime,
            string language,
            ICollection<Guid> speakerUserIds)
        {
            if (startTime > endTime)
            {
                throw new BusinessException(EventHubErrorCodes.SessionEndTimeCantBeEarlierThanStartTime);
            }

            var session = Sessions.Single(x => x.Id == sessionId);
            
            if (session.StartTime != startTime)
            {
                CheckIfValidSessionTime(endTime, sessionId);
            }

            if (session.EndTime != endTime)
            {
                CheckIfValidSessionTime(endTime, sessionId);
            }
            
            session.SetTitle(title);
            session.SetDescription(description);
            session.SetTime(startTime, endTime);
            session.SetLanguage(language);

            if (!speakerUserIds.Any())
            {
                session.RemoveSpeakers(session.Speakers.Select(x => x.UserId).ToList());
                return this;
            }
            
            var speakersToBeRemoved = session.Speakers.Where(x => speakerUserIds.Any(s => s != x.UserId)).Select(x => x.UserId).ToList();
            session.RemoveSpeakers(speakersToBeRemoved);
            session.AddSpeakers(speakerUserIds);
            return this;
        }
        
        internal Track RemoveSession(Guid sessionId)
        {
            var session = Sessions.SingleOrDefault(x => x.Id == sessionId);
            if (session is null)
            {
                throw new BusinessException(EventHubErrorCodes.SessionNotFound);
            }
            
            Sessions.Remove(session);

            return this;
        }
        
        private void CheckIfValidSessionTime(DateTime date, Guid? sessionId = null)
        {
            foreach (var session in Sessions)
            {
                if (sessionId.HasValue && sessionId!.Value == session.Id)
                {
                    continue;
                }
                
                if (date.IsBetween(session.StartTime, session.EndTime))
                {
                    throw new BusinessException(EventHubErrorCodes.SessionTimeConflictsWithAnExistingSession);
                }
            }
        }
    }
}
