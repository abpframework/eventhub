using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace EventHub.Organizations.Plans;

public class PlanInfoDefinitionStore : IPlanInfoDefinitionStore, ITransientDependency
{
    protected PlanInfoOptions PlanInfoOptions { get; }

    public PlanInfoDefinitionStore(IOptions<PlanInfoOptions> planInfoOptions)
    {
        PlanInfoOptions = planInfoOptions.Value;
    }

    public async Task<List<PlanInfoDefinition>> GetPlanInfosAsync()
    {
        return await Task.FromResult(PlanInfoOptions.Infos);
    }

    public async Task<PlanInfoDefinition> GetPlanInfoByTypeAsync(OrganizationPlanType type)
    {
        return await Task.FromResult(PlanInfoOptions.Infos.SingleOrDefault(x => x.PlanType == type));
    }
}
