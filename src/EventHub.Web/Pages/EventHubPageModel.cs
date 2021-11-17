using System;
using EventHub.Localization;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EventHub.Web.Pages
{
    public abstract class EventHubPageModel : AbpPageModel
    {
        protected EventHubPageModel()
        {
            LocalizationResourceType = typeof(EventHubResource);
        }

        protected void ShowAlert(Exception exception)
        {
            Logger.LogException(exception);
            var errorInfoConverter = LazyServiceProvider.LazyGetRequiredService<IExceptionToErrorInfoConverter>();
            var errorInfo = errorInfoConverter.Convert(exception, options =>
            {
                options.SendExceptionsDetailsToClients = false;
                options.SendStackTraceToClients = false;
            });
            Alerts.Danger(errorInfo.Message);
        }
    }
}
