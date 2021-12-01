using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Events
{
    public class EventDetailDto : EntityDto<Guid>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsOnline { get; set; }

        public string OnlineLink { get; set; }

        public Guid? CountryId { get; set; }

        public string City { get; set; }

        public string Language { get; set; }

        public int? Capacity { get; set; }
    }
}
