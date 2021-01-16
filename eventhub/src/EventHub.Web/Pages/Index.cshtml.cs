using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace EventHub.Web.Pages
{
    public class IndexModel : EventHubPageModel
    {
        public void OnGet()
        {
            
        }

        public async Task OnPostLoginAsync()
        {
            await HttpContext.ChallengeAsync("oidc");
        }
    }
}