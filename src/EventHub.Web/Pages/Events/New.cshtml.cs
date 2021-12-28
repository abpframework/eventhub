using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Users;

namespace EventHub.Web.Pages.Events
{
    public class NewPageModel : EventHubPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string EventUrlCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsCreateNew { get; set; }
        
        public bool IsHasOrganizations { get; set; }
        
        public List<EventInListDto> DraftEventList { get; set; }

        private readonly IEventAppService _eventAppService;
        private readonly IOrganizationAppService _organizationAppService;

        public NewPageModel(
            IEventAppService eventAppService, 
            IOrganizationAppService organizationAppService)
        {
            _eventAppService = eventAppService;
            _organizationAppService = organizationAppService;

            DraftEventList = new List<EventInListDto>();
        }

        public async Task OnGetAsync()
        {
            IsHasOrganizations = (await _organizationAppService.GetOrganizationsByUserIdAsync(CurrentUser.GetId())).Items.Any();

            if (!EventUrlCode.IsNullOrWhiteSpace())
            {
                return;
            }

            if (IsHasOrganizations && !IsCreateNew)
            {
                DraftEventList = await _eventAppService.GetDraftEventsByCurrentUser();
            }
        }
    }
}
