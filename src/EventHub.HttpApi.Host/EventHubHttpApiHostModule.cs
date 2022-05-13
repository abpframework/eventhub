using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using EventHub.EntityFrameworkCore;
using EventHub.Events;
using EventHub.Options;
using EventHub.Organizations;
using EventHub.Organizations.Plans;
using EventHub.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Timing;
using Volo.Abp.VirtualFileSystem;

namespace EventHub
{
    [DependsOn(
        typeof(EventHubApplicationModule),
        typeof(EventHubHttpApiModule),
        typeof(EventHubEntityFrameworkCoreModule),
        typeof(AbpAutofacModule),
        typeof(AbpCachingStackExchangeRedisModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule)
    )]
    public class EventHubHttpApiHostModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            ConfigureAuthentication(context, configuration);
            ConfigureLocalization();
            ConfigureCache(configuration);
            ConfigureVirtualFileSystem(context);
            ConfigureRedis(context, configuration);
            ConfigureCors(context, configuration);
            ConfigureCookies(context);
            ConfigureSwaggerServices(context, configuration);
            ConfigureBackgroundJobs();
            ConfigureAutoApiControllers();
            ConfigureTiming();
            ConfigurePremiumPlanInfo(context, configuration);
        }
        
        private void ConfigureAutoApiControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(CreateOrganizationDto));
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(UpdateOrganizationDto));
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(CreateEventDto));
                options.ConventionalControllers.FormBodyBindingIgnoredTypes.Add(typeof(UpdateEventDto));
            });
        }

        private void ConfigureBackgroundJobs()
        {
            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = false;
            });
        }

        private void ConfigureCache(IConfiguration configuration)
        {
            Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "EventHub:"; });
        }

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<EventHubHttpApiHostModule>(
                    baseNamespace: "EventHub",
                    baseFolder: "/Images");

                if (hostingEnvironment.IsDevelopment())
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubDomainSharedModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}EventHub.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubDomainModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}EventHub.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubApplicationContractsModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}EventHub.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<EventHubApplicationModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}EventHub.Application"));
                }
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.Audience = "EventHub";
                });
        }

        private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAbpSwaggerGenWithOAuth(
                EventHubUrlOptions.GetAccountConfigValue(configuration),
                new Dictionary<string, string>
                {
                    {"EventHub", "EventHub API"}
                },
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo {Title = "EventHub API", Version = "v1"});
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                });
        }

        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("en", "en", "English", "gb"));
            });
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

        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            EventHubUrlOptions.GetWwwConfigValue(configuration)
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        private void ConfigureCookies(ServiceConfigurationContext context)
        {
            context.Services.AddSameSiteCookiePolicy();
        }
        
        private void ConfigureTiming()
        {
            Configure<AbpClockOptions>(options => { options.Kind = DateTimeKind.Utc; });
        }
        
        private void ConfigurePremiumPlanInfo(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddOptions<List<PlanInfoDefinition>>()
                .Bind(configuration.GetSection(PlanInfoDefinition.PlanInfo))
                .ValidateDataAnnotations()
                .Validate(PlanInfoDefinition.IsValid, "PlanInfoDefinition is not valid!");

            var planInfos = context.Services.GetRequiredServiceLazy<IOptions<List<PlanInfoDefinition>>>();
            Configure<PlanInfoOptions>(options =>
            {
                options.AddPlanInfos(planInfos.Value.Value);
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });

            var supportedCultures = new[]
            {
                new CultureInfo("en")
            };
            
            app.UseAbpRequestLocalization(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
            });

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
            app.UseAuthorization();
            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "EventHub API");

                var configuration = context.GetConfiguration();
                options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
                options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
            });
            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseUnitOfWork();
            app.UseConfiguredEndpoints();
        }
    }
}
