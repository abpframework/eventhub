using System;

namespace EventHub.Organizations.Memberships
{
    public class OrganizationMemberDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}