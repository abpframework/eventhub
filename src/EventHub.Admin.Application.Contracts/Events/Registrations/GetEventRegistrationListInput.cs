using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Events.Registrations
{
    public class GetEventRegistrationListInput : PagedAndSortedResultRequestDto
    {
        public Guid EventId { get; set; }
    }
}
