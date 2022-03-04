using Volo.Abp.TextTemplating;

namespace EventHub.Emailing
{
    public class EmailTemplateDefinitionProvider : TemplateDefinitionProvider
    {
        public override void Define(ITemplateDefinitionContext context)
        {
            context.Add(
                new TemplateDefinition(
                    EmailTemplates.Layout,
                    isLayout: true
                ).WithVirtualFilePath(
                    "/Emailing/Templates/Layout.tpl",
                    isInlineLocalized: true
                )
            );
            
            context.Add(
                new TemplateDefinition(
                    EmailTemplates.EventRegistration, 
                    layout: EmailTemplates.Layout
                ).WithVirtualFilePath(
                    "/Emailing/Templates/EventRegistration.tpl",
                    isInlineLocalized: true
                )
            );
            
            context.Add(
                new TemplateDefinition(
                    EmailTemplates.EventReminder,
                    layout: EmailTemplates.Layout
                ).WithVirtualFilePath(
                    "/Emailing/Templates/EventReminder.tpl",
                    isInlineLocalized: true
                )
            );
            
            context.Add(
                new TemplateDefinition(
                    EmailTemplates.NewEventCreated,
                    layout: EmailTemplates.Layout
                ).WithVirtualFilePath(
                    "/Emailing/Templates/NewEventCreated.tpl",
                    isInlineLocalized: true
                )
            );

            context.Add(
                new TemplateDefinition(
                        EmailTemplates.EventTimingChanged,
                        layout: EmailTemplates.Layout)
                    .WithVirtualFilePath(
                        "/Emailing/Templates/EventTimingChanged.tpl",
                        isInlineLocalized: true
                    )
            );
            
            context.Add(
                new TemplateDefinition(
                        EmailTemplates.PaymentRequestCompleted)
                    .WithVirtualFilePath(
                        "/Emailing/Templates/PaymentRequestCompleted.tpl",
                        isInlineLocalized: true
                    )
            );
            
            context.Add(
                new TemplateDefinition(
                        EmailTemplates.PaymentRequestFailed)
                    .WithVirtualFilePath(
                        "/Emailing/Templates/PaymentRequestFailed.tpl",
                        isInlineLocalized: true
                    )
            );
            
            context.Add(
                new TemplateDefinition(
                        EmailTemplates.PaidEnrollmentEndDateReminder)
                    .WithVirtualFilePath(
                        "/Emailing/Templates/PaidEnrollmentEndDateReminder.tpl",
                        isInlineLocalized: true
                    )
            );
        }
    }
}
