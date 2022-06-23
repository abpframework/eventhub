using System.IO;
using System.Security.Cryptography.X509Certificates;
using EventHub.EntityFrameworkCore;
using EventHub.Localization;
using EventHub.Options;
using EventHub.Utils;
using EventHub.Web;
using EventHub.Web.Theme;
using EventHub.Web.Theme.Bundling;
using IdentityServer4.Configuration;
using IdentityServer4.Extensions;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.IdentityServer;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace EventHub
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(AbpAccountWebIdentityServerModule),
        typeof(AbpAccountApplicationModule),
        typeof(AbpAccountHttpApiModule), 
        typeof(EventHubWebThemeModule),
        typeof(EventHubEntityFrameworkCoreModule),
        typeof(AbpAspNetCoreSerilogModule)
        )]
    public class EventHubIdentityServerModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (!hostingEnvironment.IsDevelopment())
            {
                var configuration = context.Services.GetConfiguration();

                PreConfigure<AbpIdentityServerBuilderOptions>(options =>
                {
                    options.AddDeveloperSigningCredential = false;
                });

                PreConfigure<IIdentityServerBuilder>(builder =>
                {
                    builder.AddSigningCredential(GetSigningCertificate(hostingEnvironment, configuration));
                });
            }
        }
        
        private X509Certificate2 GetSigningCertificate(IWebHostEnvironment hostingEnv, IConfiguration configuration)
        {
            var fileName = "account.openeventhub.pfx";
            var passPhrase = "a8202f07-66e5-4619-be07-72ba76fde97f";
            var file = Path.Combine(hostingEnv.ContentRootPath, fileName);

            if (!File.Exists(file))
            {
                throw new FileNotFoundException($"Signing Certificate couldn't found: {file}");
            }

            return new X509Certificate2(file, passPhrase);
        }
        
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<EventHubResource>()
                    .AddBaseTypes(
                        typeof(AbpUiResource)
                    );

                options.Languages.Add(new LanguageInfo("en", "en", "English"));
            });

            Configure<AbpBundlingOptions>(options =>
            {
                options.StyleBundles.Configure(
                    EventHubThemeBundles.Styles.Global,
                    bundle =>
                    {
                        bundle.AddFiles("/global-styles.css");
                    }
                );

                options.Mode = BundlingMode.None;
            });

            Configure<AbpAuditingOptions>(options =>
            {
                //options.IsEnabledForGetRequests = true;
                options.ApplicationName = "AuthServer";
            });

            Configure<IdentityServerOptions>(options =>
            {
                options.IssuerUri = EventHubUrlOptions.GetAccountConfigValue(configuration);
            });

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}EventHub.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}EventHub.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubWebThemeModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}EventHub.Web.Theme"));
                });
            }

            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = EventHubUrlOptions.GetAccountConfigValue(configuration);
                options.RedirectAllowedUrls.AddRange(
                    new[]
                    {
                        EventHubUrlOptions.GetWwwConfigValue(configuration),
                        EventHubUrlOptions.GetApiConfigValue(configuration),
                        EventHubUrlOptions.GetAdminApiConfigValue(configuration)
                    });
            });

            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = false;
            });

            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.KeyPrefix = "EventHub:";
            });

            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            context.Services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "EventHub-Protection-Keys");

            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            EventHubUrlOptions.GetWwwConfigValue(configuration),
                            EventHubUrlOptions.GetApiConfigValue(configuration),
                            EventHubUrlOptions.GetAdminConfigValue(configuration),
                            EventHubUrlOptions.GetAdminApiConfigValue(configuration)
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            
            context.Services.AddSameSiteCookiePolicy(); 
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

            app.Use(async (ctx, next) =>
            {
                if (ctx.Request.Headers.ContainsKey("from-ingress"))
                {
                    ctx.SetIdentityServerOrigin(EventHubUrlOptions.GetAccountConfigValue(configuration));
                }
                
                await next();
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }
            
            app.UseCookiePolicy();
            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(DefaultCorsPolicyName);
            app.UseAuthentication();
            app.UseUnitOfWork();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
        }
    }
}
