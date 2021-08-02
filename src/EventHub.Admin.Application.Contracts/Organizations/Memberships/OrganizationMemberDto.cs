using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Organizations.Memberships
{
    public class OrganizationMemberDto : EntityDto<Guid>
    {
        public string OrganizationName { get; set; }
        
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}