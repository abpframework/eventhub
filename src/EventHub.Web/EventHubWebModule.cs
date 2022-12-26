using System;
using System.IO;
using EventHub.Localization;
using EventHub.Options;
using EventHub.Web.Menus;
using EventHub.Web.Theme;
using EventHub.Web.Theme.Bundling;
using EventHub.Web.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Payment.Web;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.CmsKit.Public.Web;

namespace EventHub.Web
{
    [DependsOn(
        typeof(EventHubHttpApiModule),
        typeof(EventHubHttpApiClientModule),
        typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule),
        typeof(AbpAspNetCoreMvcClientModule),
        typeof(EventHubWebThemeModule),
        typeof(AbpAutofacModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(AbpHttpClientIdentityModelWebModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpAccountApplicationContractsModule),
        typeof(CmsKitPublicWebModule),
        typeof(PaymentWebModule)
        )]
    public class EventHubWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(
                    typeof(EventHubResource),
                    typeof(EventHubDomainSharedModule).Assembly,
                    typeof(EventHubApplicationContractsModule).Assembly,
                    typeof(EventHubWebModule).Assembly
                );
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            ConfigureBundles();
            ConfigureCache(configuration);
            ConfigureRedis(context, configuration);
            ConfigureUrls(configuration);
            ConfigureAuthentication(context, configuration);
            ConfigureAutoMapper();
            ConfigureVirtualFileSystem(hostingEnvironment);
            ConfigureNavigationServices(configuration);
            ConfigureCookies(context);
            ConfigureSwaggerServices(context.Services);
            ConfigureRazorPageOptions();
        }
        
        private void ConfigureBundles()
        {
            Configure<AbpBundlingOptions>(options =>
            {
                options.StyleBundles.Configure(
                    EventHubThemeBundles.Styles.Global,
                    bundle =>
                    {
                        bundle.AddFiles("/global-styles.css");
                    }
                );
            });
        }

        private void ConfigureCache(IConfiguration configuration)
        {
            Configure<AbpDistributedCacheOptions>(options =>
            {
                options.KeyPrefix = "EventHub:";
            });
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = EventHubUrlOptions.GetWwwConfigValue(configuration);
            });
            
            Configure<AbpRemoteServiceOptions>(options =>
            {
                options.RemoteServices.Default = new RemoteServiceConfiguration(
                    EventHubUrlOptions.GetApiInternalConfigValue(configuration)
                        .EnsureEndsWith('/')
                    );
            });
        }
        
        private void ConfigureRazorPageOptions()
        {
            Configure<RazorPagesOptions>(options =>
            {
                options.Conventions.AuthorizeFolder("/Payment");

                options.Conventions.AuthorizePage("/Events/New");
                options.Conventions.AuthorizePage("/Events/Edit");
                options.Conventions.AuthorizePage("/Organizations/New");
                options.Conventions.AuthorizePage("/Organizations/Edit");
                options.Conventions.AuthorizePage("/Pricing");
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies", options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(365);
                })
                .AddAbpOpenIdConnect("oidc", options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                    options.ClientId = configuration["AuthServer:ClientId"];
                    options.ClientSecret = configuration["AuthServer:ClientSecret"];

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("role");
                    options.Scope.Add("email");
                    options.Scope.Add("phone");
                    options.Scope.Add("EventHub");
                });
        }
        
        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<EventHubWebModule>();
            });
        }

        private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}EventHub.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}EventHub.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubWebThemeModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}EventHub.Web.Theme"));
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubWebModule>(hostingEnvironment.ContentRootPath);
                });
            }
        }

        private void ConfigureNavigationServices(IConfiguration configuration)
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new EventHubMenuContributor(configuration));
            });

            Configure<AbpToolbarOptions>(options =>
            {
                options.Contributors.Add(new EventHubToolbarContributor());
            });
        }
        
        private void ConfigureCookies(ServiceConfigurationContext context)
        {
            context.Services.AddSameSiteCookiePolicy();
        }

        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EventHub API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                }
            );
        }

        private void ConfigureRedis(
            ServiceConfigurationContext context,
            IConfiguration configuration)
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            context.Services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "EventHub-Protection-Keys");
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "EventHub API");
            });
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseConfiguredEndpoints();
        }
    }
}
