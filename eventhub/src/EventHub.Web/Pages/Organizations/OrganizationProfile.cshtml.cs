using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventHub.Web.Pages.Organizations
{
    public class OrganizationProfile : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        public async Task OnGetAsync()
        {

        }
    }
}
