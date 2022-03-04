using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace EventHub.Organizations.Plans;

public class FeatureOfPlanDefinition
{
    [CanBeNull]
    [Range(0, uint.MaxValue)]
    public uint? MaxAllowedEventsCountInOneYear { get; set; }

    [CanBeNull]
    [Range(0, uint.MaxValue)]
    public uint? MaxAllowedTracksCountInOneEvent { get; set; }

    [CanBeNull]
    [Range(0, uint.MaxValue)]
    public uint? MaxAllowedAttendeesCountInOneEvent { get; set; }

    [NotNull]
    public List<string> AdditionalFeatureInfos { get; set; }

    public FeatureOfPlanDefinition()
    {
        AdditionalFeatureInfos = new List<string>();
    }
}
