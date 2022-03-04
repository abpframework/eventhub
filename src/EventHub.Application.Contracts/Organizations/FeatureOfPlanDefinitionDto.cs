using System.Collections.Generic;

namespace EventHub.Organizations;

public class FeatureOfPlanDefinitionDto
{
    public uint? MaxAllowedEventsCountInOneYear { get; set; }

    public uint? MaxAllowedTracksCountInOneEvent { get; set; }

    public uint? MaxAllowedAttendeesCountInOneEvent { get; set; }
    
    public List<string> AdditionalFeatureInfos { get; set; }

    public FeatureOfPlanDefinitionDto()
    {
        AdditionalFeatureInfos = new List<string>();
    }
}
