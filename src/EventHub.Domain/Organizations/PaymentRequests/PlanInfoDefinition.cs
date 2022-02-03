using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace EventHub.Organizations.PaymentRequests;

public class PlanInfoDefinition
{
    public const string PlanInfo = "PlanInfos";
    
    public OrganizationPlanType PlanType { get; set; }

    public string Description { get; set; }
    
    public bool IsActive { get; set; }

    [Range(1.0, 100.0, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public decimal Price { get; set; } = 1.0M;

    public bool IsExtendable { get; set; }
    
    [CanBeNull]
    [Range(0, 12, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int? CanBeExtendedAfterHowManyMonths { get; set; }

    [CanBeNull]
    [Range(1, 24, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int? OnePremiumPeriodAsMonth { get; set; } = 12;
    
    public FeatureOfPlanDefinition Feature { get; set; }

    public PlanInfoDefinition()
    {
        Feature = new FeatureOfPlanDefinition();
        Feature.AdditionalFeatureInfos = new List<string>();
    }
}
