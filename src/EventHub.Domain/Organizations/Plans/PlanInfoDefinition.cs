using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp;

namespace EventHub.Organizations.Plans;

public class PlanInfoDefinition
{
    public const string PlanInfo = "PlanInfos";
    
    public OrganizationPlanType PlanType { get; set; }

    public string Description { get; set; }
    
    public bool IsActive { get; set; }

    [Range(1.0, 100.0)]
    public decimal Price { get; set; } = 1.0M;

    public bool IsExtendable { get; set; }
    
    [CanBeNull]
    [Range(0, 12)]
    public int? CanBeExtendedAfterHowManyMonths { get; set; }

    [CanBeNull]
    [Range(1, 24)]
    public int? OnePaidEnrollmentPeriodAsMonth { get; set; } = 12;
    
    public FeatureOfPlanDefinition Feature { get; set; }

    public PlanInfoDefinition()
    {
        Feature = new FeatureOfPlanDefinition
        {
            MaxAllowedEventsCountInOneYear = uint.MaxValue,
            MaxAllowedTracksCountInOneEvent = uint.MaxValue,
            MaxAllowedAttendeesCountInOneEvent = uint.MaxValue,
            AdditionalFeatureInfos = new List<string>()
        };
    }

    public static bool IsValid(List<PlanInfoDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            var isValidPlanType = Enum.IsDefined(typeof(OrganizationPlanType), definition.PlanType);
            if (!isValidPlanType)
            {
                return false;
            }

            var isExistSamePlan = definitions.Count(x => x.Price == definition.Price && x.PlanType == definition.PlanType);
            if (isExistSamePlan > 1)
            {
                return false;
            }
                        
            if (definition.IsActive && definition.IsExtendable)
            {
                Check.NotNull(definition.OnePaidEnrollmentPeriodAsMonth, nameof(definition.OnePaidEnrollmentPeriodAsMonth));
                Check.NotNull(definition.CanBeExtendedAfterHowManyMonths, nameof(definition.CanBeExtendedAfterHowManyMonths));
                return definition.OnePaidEnrollmentPeriodAsMonth > definition.CanBeExtendedAfterHowManyMonths;
            }
        }

        return true;
    }
}
