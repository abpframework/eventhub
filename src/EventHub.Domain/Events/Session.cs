using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace EventHub.Events
{
    public class Session : Entity<Guid>
    {
        public Guid TrackId { get; private set; }

        public string Title { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime EndTime { get; private set; }

        public string Description { get; private set; }

        public string Language { get; set; }
        
        public ICollection<Speaker> Speakers { get; private set; }
        
        private Session()
        {
            
        }
        
        internal Session(
            Guid id,
            Guid trackId,
            string title,
            DateTime startTime,
            DateTime endTime,
            string description,
            string language)
            : base(id)
        {
            TrackId = trackId;
            SetTitle(title);
            SetDescription(description);
            SetTime(startTime, endTime);
            SetLanguage(language);
            Speakers = new Collection<Speaker>();
        }
        
        public Session SetTitle(string title)
        {
            Title = Check.NotNullOrWhiteSpace(title, nameof(title), SessionConsts.MaxTitleLength, SessionConsts.MinTitleLength);
            return this;
        }

        public Session SetDescription(string description)
        {
            Description = Check.NotNullOrWhiteSpace(description, nameof(description), SessionConsts.MaxDescriptionLength, SessionConsts.MinDescriptionLength);
            return this;
        }
        
        internal Session SetTime(DateTime startTime, DateTime endTime)
        {
            if (startTime > endTime)
            {
                throw new BusinessException(EventHubErrorCodes.SessionEndTimeCantBeEarlierThanStartTime);
            }

            StartTime = startTime;
            EndTime = endTime;
            return this;
        }
        
        internal Session SetLanguage(string language)
        {
            Language = Check.NotNullOrWhiteSpace(language, nameof(language), SessionConsts.MaxLanguageLength);

            return this;
        }
    }
}
