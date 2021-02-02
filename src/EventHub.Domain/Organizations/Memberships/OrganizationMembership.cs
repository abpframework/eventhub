using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace EventHub.Organizations.Memberships
{
    public class OrganizationMembership : CreationAuditedAggregateRoot<Guid>
    {
        public Guid OrganizationId { get; private set; }

        public Guid UserId { get; private set; }

        private OrganizationMembership()
        {
            
        }

        internal OrganizationMembership(
            Guid id,
            Guid organizationId,
            Guid userId)
            : base(id)
        {
            OrganizationId = organizationId;
            UserId = userId;
        }
    }
}