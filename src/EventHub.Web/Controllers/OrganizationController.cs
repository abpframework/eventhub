using System.Linq;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class OrganizationController : AbpController
    {
        private readonly IOrganizationAppService _organizationAppService;

        public OrganizationController(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetList(OrganizationListFilterDto input)
        {
            ViewData.Model = (await _organizationAppService.GetListAsync(input)).Items.ToList();
            
            return new PartialViewResult
            {
                ViewName = "~/Pages/Organizations/Components/OrganizationsArea/_organizationListSection.cshtml",
                ViewData = ViewData
            };
        }
    }
}
