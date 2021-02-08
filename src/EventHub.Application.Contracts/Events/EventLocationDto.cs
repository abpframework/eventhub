using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Events
{
    public class EventLocationDto : EntityDto<Guid>
    {
        public bool IsOnline { get; set; }

        public string Link { get; set; }

        public bool IsRegistered { get; set; }
    }
}