using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Events
{
    public class EventListFilterDto : PagedResultRequestDto
    {
        public DateTime? MinDate { get; set; }

        public DateTime? MaxDate { get; set; }

        public EventListFilterDto()
        {
            MaxResultCount = 20;
        }
    }
}