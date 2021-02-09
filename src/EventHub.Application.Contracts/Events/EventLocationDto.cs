using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Events
{
    public class EventLocationDto : EntityDto<Guid>
    {
        public bool IsOnline { get; set; }

        public bool IsRegistered { get; set; }

        public string OnlineLink { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

    }
}