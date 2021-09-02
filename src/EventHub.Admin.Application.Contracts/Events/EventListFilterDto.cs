using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Events
{
    public class EventListFilterDto : PagedAndSortedResultRequestDto
    {
        public DateTime? MinStartTime { get; set; }
        public DateTime? MaxStartTime { get; set; }
        public string Title { get; set; }
        public string OrganizationDisplayName { get; set; }
        public int? MinAttendeeCount { get; set; }
        public int? MaxAttendeeCount { get; set; }
    }
}
