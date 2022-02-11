using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Payment;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.IdentityServer;
using Volo.Abp.SettingManagement;
using Volo.Abp.VirtualFileSystem;
using Volo.CmsKit;
using Volo.CmsKit.Comments;
using EventHub.Events;
using Volo.CmsKit.Tags;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubDomainSharedModule),
        typeof(AbpAuditLoggingDomainModule),
        typeof(AbpBackgroundJobsDomainModule),
        typeof(AbpIdentityDomainModule),
        typeof(AbpPermissionManagementDomainIdentityModule),
        typeof(AbpIdentityServerDomainModule),
        typeof(AbpPermissionManagementDomainIdentityServerModule),
        typeof(AbpSettingManagementDomainModule),
        typeof(AbpEmailingModule),
        typeof(BlobStoringDatabaseDomainModule),
        typeof(CmsKitDomainModule),
        typeof(PaymentDomainModule)
    )]
    public class EventHubDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<EventHubDomainModule>();
            });
            
            Configure<CmsKitCommentOptions>(options =>
            {
                options.EntityTypes.Add(new CommentEntityTypeDefinition(nameof(Event)));
            });
            
            Configure<CmsKitTagOptions>(options =>
            {
                options.EntityTypes.Add(new TagEntityTypeDefiniton(nameof(Event)));
            });
            
#if DEBUG
            context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
        }
    }
}
