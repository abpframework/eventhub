using System;
using EventHub.Organizations;
using EventHub.Organizations.Plans;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace EventHub
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAuthorizationModule),
        typeof(EventHubDomainModule)
        )]
    public class EventHubTestBaseModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<AbpIdentityServerBuilderOptions>(options =>
            {
                options.AddDeveloperSigningCredential = false;
            });

            PreConfigure<IIdentityServerBuilder>(identityServerBuilder =>
            {
                identityServerBuilder.AddDeveloperSigningCredential(false, Guid.NewGuid().ToString());
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = false;
            });

            context.Services.AddAlwaysAllowAuthorization();
            
            Configure<PlanInfoOptions>(options =>
            {
                options.Infos.Add(new PlanInfoDefinition
                {
                    PlanType = OrganizationPlanType.Free,
                    Description = "This is a default plan.",
                    IsActive = true,
                    Price = 0,
                    IsExtendable = false
                });
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            SeedTestData(context);
        }

        private static void SeedTestData(ApplicationInitializationContext context)
        {
            AsyncHelper.RunSync(async () =>
            {
                using (var scope = context.ServiceProvider.CreateScope())
                {
                    await scope.ServiceProvider
                        .GetRequiredService<IDataSeeder>()
                        .SeedAsync();
                }
            });
        }
    }
}
