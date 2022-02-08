using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace EventHub.Organizations.Plans;

public class FeatureOfPlanDefinition
{
    [CanBeNull]
    [Range(0, uint.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public uint? MaxAllowedEventsCountInOneYear { get; set; }

    [CanBeNull]
    [Range(0, uint.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public uint? MaxAllowedTracksCountInOneEvent { get; set; }

    [CanBeNull]
    [Range(0, uint.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public uint? MaxAllowedAttendeesCountInOneEvent { get; set; }

    [NotNull]
    public List<string> AdditionalFeatureInfos { get; set; }
}
