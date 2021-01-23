using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace EventHub.Events
{
    public class Event : FullAuditedAggregateRoot<Guid>
    {
        public Guid OrganizationId { get; private set; }

        public string Title { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime EndTime { get; private set; }

        public string Description { get; private set; }

        public bool IsOnline { get; set; }

        public int? Capacity { get; set; }

        private Event()
        {

        }

        internal Event(
            Guid id,
            Guid organizationId,
            string title,
            DateTime startTime,
            DateTime endTime,
            string description)
        : base(id)
        {
            OrganizationId = organizationId;
            SetTitle(title);
            SetDescription(description);
            SetTime(startTime, endTime);
        }

        public Event SetTitle(string title)
        {
            Title = Check.NotNullOrWhiteSpace(title, nameof(title), EventConsts.MaxTitleLength, EventConsts.MinTitleLength);
            return this;
        }

        public Event SetDescription(string description)
        {
            Description = Check.NotNullOrWhiteSpace(description, nameof(description), EventConsts.MaxDescriptionLength, EventConsts.MinDescriptionLength);
            return this;
        }

        public Event SetTime(DateTime startTime, DateTime endTime)
        {
            if (startTime > endTime)
            {
                throw new BusinessException(EventHubErrorCodes.EventEndTimeCantBeEarlierThanStartTime);
            }

            StartTime = startTime;
            EndTime = endTime;
            return this;
        }
    }
}
