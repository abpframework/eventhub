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
        public IReadOnlyList<EventInListDto> UpcomingEvents { get; private set; }
        public long UpcomingEventTotalCount { get; private set; }

        public IReadOnlyList<EventInListDto> PastEvents { get; private set; }
        public long PastEventTotalCount { get; private set; }

        public bool IsOrganizationOwner { get; private set; }

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
            await GetProfileAsync();
            await GetUpcomingEventsAsync();
            await GetPastEventsAsync();

            IsOrganizationOwner = await _organizationAppService.IsOrganizationOwnerAsync(Organization.Id);
        }

        private async Task GetProfileAsync()
        {
            Organization = await _organizationAppService.GetProfileAsync(Name);
        }

        private async Task GetUpcomingEventsAsync()
        {
            var result = await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    MinDate = Clock.Now,
                    OrganizationId = Organization.Id
                }
            );
            UpcomingEvents = result.Items;
            UpcomingEventTotalCount = result.TotalCount;
        }

        private async Task GetPastEventsAsync()
        {
            var result = await _eventAppService.GetListAsync(
                new EventListFilterDto
                {
                    MaxDate = Clock.Now,
                    OrganizationId = Organization.Id
                }
            );
            PastEvents = result.Items;
            PastEventTotalCount = result.TotalCount;
        }
    }
}
