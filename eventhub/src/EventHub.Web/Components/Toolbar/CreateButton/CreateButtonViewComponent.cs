using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Web.Components.Toolbar.CreateButton
{
    public class CreateButtonViewComponent : AbpViewComponent
    {
        public virtual IViewComponentResult Invoke()
        {
            return View("~/Components/Toolbar/CreateButton/Default.cshtml");
        }
    }
}
