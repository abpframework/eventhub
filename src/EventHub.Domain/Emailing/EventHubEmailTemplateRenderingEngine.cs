using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Web;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TextTemplating;

namespace EventHub.Emailing
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(ITemplateRenderer), typeof(TemplateRenderer), typeof(EventHubEmailTemplateRenderingEngine))]
    public class EventHubEmailTemplateRenderingEngine : TemplateRenderer
    {
        public EventHubEmailTemplateRenderingEngine(ITemplateContentProvider templateContentProvider, ITemplateDefinitionManager templateDefinitionManager, IStringLocalizerFactory stringLocalizerFactory) : base(templateContentProvider, templateDefinitionManager, stringLocalizerFactory)
        {
        }

        public override async Task<string> RenderAsync(
            [NotNull] string templateName,
            [CanBeNull] object model = null,
            [CanBeNull] string cultureName = null,
            [CanBeNull] Dictionary<string, object> globalContext = null)
        {
            if (globalContext == null)
            {
                globalContext = new Dictionary<string, object>();
            }

            globalContext["current_year"] = DateTime.Today.Year;
            globalContext["app_url"] = EventHubExternalUrls.EhWww;

            return await base.RenderAsync(templateName, model, cultureName, globalContext);
        }
    }
}