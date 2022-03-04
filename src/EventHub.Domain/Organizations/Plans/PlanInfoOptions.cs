using System.Collections.Generic;
using System.Linq;

namespace EventHub.Organizations.Plans;

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
    
    public List<PlanInfoDefinition> GetPlanInfos()
    {
        return Infos.Where(x => x.IsActive).ToList();
    }

    public PlanInfoDefinition GetPlanInfoByType(OrganizationPlanType type)
    {
        return Infos.SingleOrDefault(x => x.IsActive && x.PlanType == type);
    }
}
