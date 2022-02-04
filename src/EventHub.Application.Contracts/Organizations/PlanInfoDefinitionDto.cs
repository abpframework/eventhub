namespace EventHub.Organizations;

public class PlanInfoDefinitionDto
{
    public OrganizationPlanType PlanType { get; set; }
    
    public string Description { get; set; }

    public bool IsActive { get; set; }

    public decimal Price { get; set; }

    public bool IsExtendable { get; set; }
    
    public int? CanBeExtendedAfterHowManyMonths { get; set; }

    public int? OnePaidEnrollmentPeriodAsMonth { get; set; }
    
    public FeatureOfPlanDefinitionDto Feature { get; set; }
    
    public PlanInfoDefinitionDto()
    {
        Feature = new FeatureOfPlanDefinitionDto();
    }
}
