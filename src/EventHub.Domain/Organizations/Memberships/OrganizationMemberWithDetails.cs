using System;
using Volo.Abp.Domain.Entities;

namespace EventHub.Organizations.Memberships
{
    public class OrganizationMemberWithDetails : Entity<Guid>
    {
        public string OrganizationName { get; set; }
        
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}