using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Organizations;
using Volo.Abp.Application.Dtos;

namespace EventHub.Web.Pages.Organizations
{
    public class IndexPageModel : EventHubPageModel
    {
        public IReadOnlyList<OrganizationInListDto> Organizations { get; set; }

        private readonly IOrganizationAppService _organizationAppService;

        public IndexPageModel(IOrganizationAppService organizationAppService)
        {
            _organizationAppService = organizationAppService;
        }

        public async Task OnGetAsync()
        {
            var result = await _organizationAppService.GetListAsync(new PagedResultRequestDto());
            Organizations = result.Items;
        }
    }
}
