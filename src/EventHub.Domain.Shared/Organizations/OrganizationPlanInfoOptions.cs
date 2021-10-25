using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Organizations
{
    public class OrganizationPlanInfoOptions
    {
        public const string OrganizationPlanInfo = "Organization:PlanInfo";

        public OrganizationPlanType PlanType { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [Range(1.0, 100.0, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public float Price { get; set; } = 1.0f;

        [DefaultValue(true)]
        public bool IsExtendable { get; set; }
        
        [Range(1, 12, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int CanBeExtendedAfterHowManyMonths { get; set; } 
        
        [Range(1, 24, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int OnePremiumPeriodAsMonth { get; set; }
    }
}