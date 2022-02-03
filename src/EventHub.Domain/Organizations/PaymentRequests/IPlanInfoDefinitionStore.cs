using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHub.Organizations.PaymentRequests;

public interface IPlanInfoDefinitionStore
{
    Task<List<PlanInfoDefinition>> GetPlanInfosAsync();
}
