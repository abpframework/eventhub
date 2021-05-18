using System;
using Volo.Abp;
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
            UrlCode = Check.NotNullOrWhiteSpace(urlCode, urlCode, EventConsts.UrlCodeLength, EventConsts.UrlCodeLength);
            SetTitle(title);
            SetDescription(description);
            SetTimeInternal(startTime, endTime);
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
            
            if (TimingChangeCount >= EventConsts.MaxTimingChangeCountForUser)
            {
                throw new BusinessException(EventHubErrorCodes.CantChangeEventTiming)
                    .WithData("MaxTimingChangeLimit", EventConsts.MaxTimingChangeCountForUser);
            }
            
            AddLocalEvent(new EventTimeChangingEventData(this, StartTime, EndTime));

            return SetTimeInternal(startTime, endTime);
        }

        private Event SetTimeInternal(DateTime startTime, DateTime endTime)
        {
            if (startTime > endTime)
            {
                throw new BusinessException(EventHubErrorCodes.EventEndTimeCantBeEarlierThanStartTime);
            }

            StartTime = startTime;
            EndTime = endTime;
            TimingChangeCount++;
            return this;
        }
    }
}
