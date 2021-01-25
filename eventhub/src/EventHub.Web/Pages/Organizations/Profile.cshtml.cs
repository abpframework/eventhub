using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Pages.Organizations
{
    public class ProfilePageModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        public OrganizationProfileDto Organization { get; set; }
        public IReadOnlyList<EventInListDto> Events { get; private set; }

        private readonly IEventAppService _eventAppService;
        private readonly IOrganizationAppService _organizationAppService;

        public ProfilePageModel(
            IOrganizationAppService organizationAppService,
            IEventAppService eventAppService)
        {
            _organizationAppService = organizationAppService;
            _eventAppService = eventAppService;
        }

        public async Task OnGetAsync()
        {
            Organization = await _organizationAppService.GetProfileAsync(Name);

            var result = await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    MinDate = Clock.Now,
                    OrganizationId = Organization.Id
                }
            );

            Events = result.Items;
        }
    }
}
