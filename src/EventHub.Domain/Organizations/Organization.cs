using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Validation;

namespace EventHub.Organizations
{
    public class Organization : FullAuditedAggregateRoot<Guid>
    {
        public Guid OwnerUserId { get; private set; }

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string Description { get; private set; }

        public string Website { get; set; }

        public string TwitterUsername { get; set; }

        public string GitHubUsername { get; set; }

        public string FacebookUsername { get; set; }

        public string InstagramUsername { get; set; }

        public string MediumUsername { get; set; }

        public int MemberCount { get; internal set; }

        public OrganizationPlanType PlanType { get; private set; }

        public DateTime? PaidEnrollmentEndDate { get; private set; }

        public bool IsSendPaidEnrollmentReminderEmail { get; set; }

        private Organization()
        {
        }

        internal Organization(
            Guid id,
            Guid ownerUserId,
            string name,
            string displayName,
            string description)
            : base(id)
        {
            OwnerUserId = ownerUserId;
            SetName(name);
            SetDisplayName(displayName);
            SetDescription(description);
            SetFreeToPlanType();
        }

        internal Organization SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(name, nameof(name), OrganizationConsts.MaxNameLength, OrganizationConsts.MinNameLength);
            return this;
        }

        public Organization SetDisplayName(string displayName)
        {
            DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), OrganizationConsts.MaxDisplayNameLength, OrganizationConsts.MinDisplayNameLength);
            return this;
        }

        public Organization SetDescription(string description)
        {
            Description = Check.NotNullOrWhiteSpace(description, nameof(description), OrganizationConsts.MaxDescriptionNameLength, OrganizationConsts.MinDescriptionNameLength);
            return this;
        }

        internal Organization UpgradeToPlanType(OrganizationPlanType planType, DateTime endDate)
        {
            SetPlanType(planType, endDate);

            return this;
        }
        
        public Organization SetFreeToPlanType()
        {
            if (PlanType == OrganizationPlanType.Free)
            {
                return this;
            }
            
            SetPlanType(OrganizationPlanType.Free);

            return this;
        }

        private Organization SetPlanType(OrganizationPlanType planType, DateTime? endDate = null)
        {
            if (!Enum.IsDefined(typeof(OrganizationPlanType), planType))
            {
                throw new AbpValidationException();
            }

            if (planType != OrganizationPlanType.Free)
            {
                Check.NotNull(endDate, nameof(endDate));
            }
            
            PlanType = planType;
            PaidEnrollmentEndDate = endDate;

            return this;
        }
    }
}
