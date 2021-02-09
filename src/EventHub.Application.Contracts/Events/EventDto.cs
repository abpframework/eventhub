using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Events
{
    public class EventDto : EntityDto<Guid>
    {
        public Guid OrganizationId { get; set; }

        public string UrlCode { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Description { get; set; }

        public bool IsOnline { get; set; }

        public string Language { get; set; }

        public int? Capacity { get; set; }
    }
}
