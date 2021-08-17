using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Users
{
    public class GetUnregisteredUserEventInput : PagedResultRequestDto
    {
        public Guid EventId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Sorting { get; set; }
    }
}
