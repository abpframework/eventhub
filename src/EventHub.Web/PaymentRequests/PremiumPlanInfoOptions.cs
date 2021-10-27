using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using EventHub.Organizations;
using Volo.Abp;

namespace EventHub.Web.PaymentRequests
{
    public class PremiumPlanInfoOptions
    {
        private const string ProductNamePrefix = "EventHub";
        
        public const string OrganizationPlanInfo = "PlanInfo:Premium";

        public OrganizationPlanType PlanType { get; private set; } = OrganizationPlanType.Premium;

        [DefaultValue(false)]
        public bool IsActive { get; set; }

        [Range(1.0, 100.0, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public decimal Price { get; set; } = 1.0M;

        [DefaultValue(false)]
        public bool IsExtendable { get; set; }
        
        [Range(0, 12, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int CanBeExtendedAfterHowManyMonths { get; set; }

        [Range(1, 24, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int OnePremiumPeriodAsMonth { get; set; } = 12;

        public static string GetProductId(PremiumPlanInfoOptions premiumPlan, bool isExtend)
        {
            var productIdStringBuilder = new StringBuilder(ProductNamePrefix);
            productIdStringBuilder.Append("-");
            productIdStringBuilder.Append(premiumPlan.PlanType.ToString());
            productIdStringBuilder.Append("-");
            
            if (isExtend)
            {
                if (!premiumPlan.IsExtendable)
                {
                    throw new BusinessException();
                }

               
                productIdStringBuilder.Append("Extend");
            }
            else
            {
                productIdStringBuilder.Append("Upgrade");
            }

            return productIdStringBuilder.ToString();
        }
        
        public static string GetProductName(PremiumPlanInfoOptions premiumPlan, string organizationName, bool isExtend)
        {
            var productNameStringBuilder = new StringBuilder(ProductNamePrefix);
            productNameStringBuilder.Append(" ");
            productNameStringBuilder.Append(premiumPlan.PlanType.ToString());
            productNameStringBuilder.Append(" ");
            
            if (isExtend)
            {
                if (!premiumPlan.IsExtendable)
                {
                    throw new BusinessException();
                }

               
                productNameStringBuilder.Append("Extend");
            }
            else
            {
                productNameStringBuilder.Append("Upgrade");
            }

            productNameStringBuilder.Append(" | ");
            productNameStringBuilder.Append(organizationName);

            return productNameStringBuilder.ToString();
        }
    }
}