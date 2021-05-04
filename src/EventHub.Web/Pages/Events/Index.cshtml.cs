using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventHub.Web.Pages.Events
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid? OrganizationId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? MinDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? MaxDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? IsOnline { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string Language { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public Guid? CountryId { get; set; }
        
        public void OnGet()
        {
            
        }
    }
}