using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace EventHub.Organizations.PaymentRequests;

public class PlanInfoDefinitionStore : IPlanInfoDefinitionStore, ITransientDependency
{
    protected PlanInfoOptions PlanInfoOptions { get; }

    public PlanInfoDefinitionStore(IOptions<PlanInfoOptions> planInfoOptions)
    {
        PlanInfoOptions = planInfoOptions.Value;
    }

    public Task<List<PlanInfoDefinition>> GetPlanInfosAsync()
    {
        return Task.FromResult(PlanInfoOptions.Infos);
    }
}
