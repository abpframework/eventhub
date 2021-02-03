using Volo.Abp.TextTemplating;

namespace EventHub.Emailing
{
    public class EmailTemplateDefinitionProvider : TemplateDefinitionProvider
    {
        public override void Define(ITemplateDefinitionContext context)
        {
            context.Add(
                new TemplateDefinition(
                    EmailTemplates.EventRegistration
                ).WithVirtualFilePath(
                    "/Emailing/Templates/EventRegistration.tpl",
                    isInlineLocalized: true
                )
            );
        }
    }
}