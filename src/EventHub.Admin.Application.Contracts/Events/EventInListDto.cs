using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Events
{
    public class EventInListDto : EntityDto<Guid>
    {
        public string Title { get; set; }

        public string OrganizationDisplayName { get; set; }

        public int AttendeeCount { get; set; }

        public DateTime StartTime { get; set; }
    }
}
