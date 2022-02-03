using System.Collections.Generic;

namespace EventHub.Organizations.PaymentRequests;

public class PlanInfoOptions
{
    public List<PlanInfoDefinition> Infos { get; }

    public PlanInfoOptions()
    {
        Infos = new List<PlanInfoDefinition>();
    }
    
    public PlanInfoOptions AddPlanInfos(List<PlanInfoDefinition> infos)
    {
        foreach (var info in infos)
        {
            AddPlanInfo(info);
        }

        return this;
    }

    private PlanInfoOptions AddPlanInfo(PlanInfoDefinition info)
    {  
        Infos.AddIfNotContains(info);

        return this;
    }
}
