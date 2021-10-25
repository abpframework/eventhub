using System.Collections.Generic;
using EventHub.Localization;
using EventHub.Organizations;
using EventHub.Web;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.BlobStoring.Database;
using Payment;

namespace EventHub
{
    [DependsOn(
        typeof(AbpAuditLoggingDomainSharedModule),
        typeof(AbpBackgroundJobsDomainSharedModule),
        typeof(AbpIdentityDomainSharedModule),
        typeof(AbpIdentityServerDomainSharedModule),
        typeof(AbpPermissionManagementDomainSharedModule),
        typeof(AbpSettingManagementDomainSharedModule),
        typeof(BlobStoringDatabaseDomainSharedModule),
        typeof(PaymentDomainSharedModule)
    )]
    public class EventHubDomainSharedModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            EventHubGlobalFeatureConfigurator.Configure();
            EventHubModuleExtensionConfigurator.Configure();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<EventHubDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<EventHubResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/EventHub");

                options.DefaultResourceType = typeof(EventHubResource);
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("EventHub", typeof(EventHubResource));
            });

            Configure<EventHubUrlOptions>(configuration.GetSection("AppUrls"));

            context.Services.AddOptions<List<OrganizationPlanInfoOptions>>()
                .Bind(configuration.GetSection(OrganizationPlanInfoOptions.OrganizationPlanInfo))
                .Validate(config =>
                {
                    foreach (var planInfo in config)
                    {
                        if (planInfo.IsActive)
                        {
                            return planInfo.OnePremiumPeriodAsMonth > planInfo.CanBeExtendedAfterHowManyMonths;
                        }
                    }

                    return true;
                }, "OnePremiumPeriodAsMonth must be greater than CanBeExtendedAfterHowManyMonths.");
        }
    }
}