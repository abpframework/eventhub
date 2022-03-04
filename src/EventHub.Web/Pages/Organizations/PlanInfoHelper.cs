using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventHub.Organizations;
using Volo.Abp;

namespace EventHub.Web.Pages.Organizations;

public static class PlanInfoHelper
{
    public static PlanInfoDefinitionDto GetPlan(OrganizationPlanType planType, List<PlanInfoDefinitionDto> plans)
    {
        return plans.Single(x => x.PlanType == planType);
    }
    
    public static string GetProductId(PlanInfoDefinitionDto premiumPlan, bool isExtend)
    {
        var productIdStringBuilder = new StringBuilder(OrganizationConsts.ProductNamePrefix);
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
        
    public static string GetProductName(PlanInfoDefinitionDto premiumPlan, string organizationName, bool isExtend)
    {
        var productNameStringBuilder = new StringBuilder(OrganizationConsts.ProductNamePrefix);
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
    
    public static string GetFeatureInfoByCount(uint? count)
    {
        count ??= uint.MaxValue;
        return count == uint.MaxValue ? "Unlimited" : count.ToString();
    }
}
