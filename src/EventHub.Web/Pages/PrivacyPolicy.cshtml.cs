using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventHub.Web.Pages
{
    public class PrivacyPolicy : PageModel
    {
        public static string LastUpdateDate { get; set; } = "February 23, 2020";  // ToDo: Change LastUpdateDate

        public void OnGet()
        {
            
        }
    }
}