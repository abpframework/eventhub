using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHub.Organizations.Plans;

public interface IPlanInfoDefinitionStore
{
    Task<List<PlanInfoDefinition>> GetPlanInfosAsync();
    
    Task<PlanInfoDefinition> GetPlanInfoByTypeAsync(OrganizationPlanType type);
}
