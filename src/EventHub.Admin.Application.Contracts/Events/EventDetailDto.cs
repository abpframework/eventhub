using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Events
{
    public class EventDetailDto : EntityDto<Guid>
    {
        public string Title { get; set; }

        public DateTime StartTime { get; set; }
    }
}
