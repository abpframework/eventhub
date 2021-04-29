using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Events
{
    public class EventInListDto : EntityDto<Guid>
    {
        public string OrganizationName { get; set; }

        public string OrganizationDisplayName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
        
        public byte[] CoverImageContent { get; set; }

        public bool IsOnline { get; set; }
        
        public string Country { get; set; }

        public string City { get; set; }

        public int? Capacity { get; set; }

        public bool IsLiveNow { get; set; }

        public string UrlCode { get; set; }

        public string Url { get; set; }
    }
}
