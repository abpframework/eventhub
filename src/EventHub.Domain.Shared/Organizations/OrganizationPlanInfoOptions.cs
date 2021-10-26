using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Volo.Abp;

namespace EventHub.Organizations
{
    public class OrganizationPlanInfoOptions
    {
        private const string ProductNamePrefix = "EventHub";
        
        public const string OrganizationPlanInfo = "Organization:PlanInfo";

        public OrganizationPlanType PlanType { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [Range(1.0, 100.0, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public decimal Price { get; set; } = 1.0M;

        [DefaultValue(true)]
        public bool IsExtendable { get; set; }
        
        [Range(1, 12, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int CanBeExtendedAfterHowManyMonths { get; set; } 
        
        [Range(1, 24, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int OnePremiumPeriodAsMonth { get; set; }

        public static string GetProductId(OrganizationPlanInfoOptions organizationPlan, bool isExtend)
        {
            var productIdStringBuilder = new StringBuilder(ProductNamePrefix);
            productIdStringBuilder.Append("-");
            productIdStringBuilder.Append(organizationPlan.PlanType.ToString());
            productIdStringBuilder.Append("-");
            
            if (isExtend)
            {
                if (!organizationPlan.IsExtendable)
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
        
        public static string GetProductName(OrganizationPlanInfoOptions organizationPlan, string organizationName, bool isExtend)
        {
            var productNameStringBuilder = new StringBuilder(ProductNamePrefix);
            productNameStringBuilder.Append(" ");
            productNameStringBuilder.Append(organizationPlan.PlanType.ToString());
            productNameStringBuilder.Append(" ");
            
            if (isExtend)
            {
                if (!organizationPlan.IsExtendable)
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