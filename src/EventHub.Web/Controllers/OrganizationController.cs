using System.Linq;
using System.Threading.Tasks;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    [Route("api/organization")]
    public class OrganizationController : AbpController
    {
        private readonly IOrganizationAppService _organizationAppService;

        public OrganizationController(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }
        
        [HttpGet]
        [Route("get-list")]
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