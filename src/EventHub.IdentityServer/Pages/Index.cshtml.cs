using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace EventHub.Pages
{
    public class IndexModel : AbpPageModel
    {
        public IActionResult OnGet()
        {
            return Redirect("/account/manage");
        }
    }
}