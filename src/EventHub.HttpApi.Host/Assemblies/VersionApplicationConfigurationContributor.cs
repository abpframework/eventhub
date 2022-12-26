using System.Threading.Tasks;
using EventHub;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Data;

public class VersionApplicationConfigurationContributor : IApplicationConfigurationContributor
{
    public Task ContributeAsync(ApplicationConfigurationContributorContext context)
    {
        var assemblyInfo = AssemblyInfoHelper.Get();
        context.ApplicationConfiguration.SetProperty("versions", new
        {
            abp = assemblyInfo.AbpCoreVersion,
            application = assemblyInfo.Version,
            applicationCompileDate = assemblyInfo.ModificationDate
        });

        return Task.CompletedTask;
    }
}