using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace EventHub.Events
{
    public class Event : FullAuditedAggregateRoot<Guid>
    {
        public Guid OrganizationId { get; private set; }

        public string UrlCode { get; private set; }

        public string Url { get; private set; }

        public string Title { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime EndTime { get; private set; }

        public string Description { get; private set; }

        public bool IsOnline { get; internal set; }
        
        public string OnlineLink { get; internal set; }

        public Guid? CountryId { get; internal set; }
        
        public string CountryName { get; internal set; }

        public string City { get; internal set; }

        public string Language { get; set; }

        public int? Capacity { get; internal set; }

        public bool IsRemindingEmailSent { get; set; }

        public bool IsEmailSentToMembers { get; set; }
        
        public int TimingChangeCount  { get; set; }
        
        public bool IsTimingChangeEmailSent { get; set; }
        
        public bool IsDraft { get; private set; }
        
        public ICollection<Track> Tracks { get; private set; }
        
        private Event()
        {

        }

        internal Event(
            Guid id,
            Guid organizationId,
            string urlCode,
            string title,
            DateTime startTime,
            DateTime endTime,
            string description)
        : base(id)
        {
            OrganizationId = organizationId;
            UrlCode = Check.NotNullOrWhiteSpace(urlCode, nameof(urlCode), EventConsts.UrlCodeLength, EventConsts.UrlCodeLength);
            
            SetTitle(title);
            SetDescription(description);
            SetTimeInternal(startTime, endTime);

            Publish(false);
            
            Tracks = new Collection<Track>();
        }

        public Event SetTitle(string title)
        {
            Title = Check.NotNullOrWhiteSpace(title, nameof(title), EventConsts.MaxTitleLength, EventConsts.MinTitleLength);
            Url = EventUrlHelper.ConvertTitleToUrlPart(Title) + "-" + UrlCode;
            return this;
        }

        public Event SetDescription(string description)
        {
            Description = Check.NotNullOrWhiteSpace(description, nameof(description), EventConsts.MaxDescriptionLength, EventConsts.MinDescriptionLength);
            return this;
        }

        public Event SetTime(DateTime startTime, DateTime endTime)
        {
            if (startTime == StartTime && endTime == EndTime)
            {
                return this;
            }

            if (!IsDraft)
            {
                if (TimingChangeCount >= EventConsts.MaxTimingChangeCountForUser)
                {
                    throw new BusinessException(EventHubErrorCodes.CantChangeEventTiming)
                        .WithData("MaxTimingChangeLimit", EventConsts.MaxTimingChangeCountForUser);
                }
            }
            
            SetTimeInternal(startTime, endTime);

            if (!IsDraft)
            {
                TimingChangeCount++;
                IsTimingChangeEmailSent = false;
            }
            
            return this;
        }

        private Event SetTimeInternal(DateTime startTime, DateTime endTime)
        {
            if (startTime > endTime)
            {
                throw new BusinessException(EventHubErrorCodes.EndTimeCantBeEarlierThanStartTime);
            }

            StartTime = startTime;
            EndTime = endTime;
            return this;
        }

        internal Event AddTrack(Guid trackId, string name)
        {
            if (Tracks.Any(x => x.Name == name))
            {
                throw new BusinessException(EventHubErrorCodes.TrackNameAlreadyExist)
                    .WithData("Name", name);
            }
            
            Tracks.Add(new Track(trackId, this.Id, name));

            return this;
        }
        
        public Event UpdateTrack(Guid trackId, string name)
        {
            if (Tracks.Any(x => x.Name == name))
            {
                throw new BusinessException(EventHubErrorCodes.TrackNameAlreadyExist)
                    .WithData("Name", name);
            }
            
            var track = Tracks.SingleOrDefault(x => x.Id == trackId);
            if (track is null)
            {
                throw new BusinessException(EventHubErrorCodes.TrackNotFound);
            }

            track.SetName(name);

            return this;
        }
        
        public Event RemoveTrack(Guid trackId)
        {
            var track = Tracks.SingleOrDefault(x => x.Id == trackId);
            if (track is null)
            {
                throw new BusinessException(EventHubErrorCodes.TrackNotFound);
            }
            
            Tracks.Remove(track);

            return this;
        }

        public Event AddSession(
            Guid trackId,
            Guid sessionId,
            string title,
            string description,
            DateTime startTime, 
            DateTime endTime,
            string language,
            ICollection<Guid> speakerUserIds)
        {
            CheckIfValidSessionTime(startTime, endTime);
            
            var track = GetTrack(trackId);
            track.AddSession(sessionId, title, description,startTime, endTime, language, speakerUserIds);
            return this;
        }

        public Event UpdateSession(
            Guid trackId,
            Guid sessionId,
            string title,
            string description,
            DateTime startTime, 
            DateTime endTime,
            string language,
            ICollection<Guid> speakerUserIds)
        {
            CheckIfValidSessionTime(startTime, endTime);

            var track = GetTrack(trackId);
            track.UpdateSession(sessionId, title, description, startTime, endTime, language, speakerUserIds);
            return this;
        }
        
        public Event RemoveSession(Guid trackId, Guid sessionId)
        {
            var track = Tracks.SingleOrDefault(x => x.Id == trackId);
            if (track is null)
            {
                throw new BusinessException(EventHubErrorCodes.TrackNotFound);
            }

            track.RemoveSession(sessionId);

            return this;
        }
        
        public Event Publish(bool isPublish = true)
        {
            IsDraft = !isPublish;

            return this;
        }

        public bool IsLive(DateTime now)
        {
            return now.IsBetween(StartTime, EndTime);
        }

        private Track GetTrack(Guid trackId)
        {
            return Tracks.FirstOrDefault(t => t.Id == trackId) ??
                   throw new EntityNotFoundException(typeof(Track), trackId);
        }
        
        private void CheckIfValidSessionTime(DateTime startTime, DateTime endTime)
        {
            if (startTime < this.StartTime || this.EndTime < endTime)
            {
                throw new BusinessException(EventHubErrorCodes.SessionTimeShouldBeInTheEventTime);
            }
        }
    }
}
