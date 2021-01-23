using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Organizations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace EventHub.Web.Pages.Events
{
    public class New : EventHubPageModel
    {
        [BindProperty]
        public NewEventViewModel Event { get; set; }

        public List<SelectListItem> Organizations { get; set; }

        private readonly IEventAppService _eventAppService;
        private readonly IOrganizationAppService _organizationAppService;

        public New(
            IEventAppService eventAppService,
            IOrganizationAppService organizationAppService)
        {
            _eventAppService = eventAppService;
            _organizationAppService = organizationAppService;
        }

        public async Task OnGetAsync()
        {
            Event = new NewEventViewModel
            {
                StartTime = DateTime.Now.ClearTime().AddDays(1).AddHours(19),
                EndTime = DateTime.Now.ClearTime().AddDays(1).AddHours(21)
            };

            await FillOrganizationsAsync();
        }

        private async Task FillOrganizationsAsync()
        {
            var result = await _organizationAppService.GetMyOrganizationsAsync();
            Organizations = result.Items.Select(
                organization => new SelectListItem
                {
                    Value = organization.Id.ToString(),
                    Text = organization.DisplayName
                }
            ).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ValidateModel();

                var input = ObjectMapper.Map<NewEventViewModel, CreateEventDto>(Event);
                await _eventAppService.CreateAsync(input);

                //TODO: Redirect to the event!
                return RedirectToPage("/");
            }
            catch (Exception exception)
            {
                ShowAlert(exception);
                await FillOrganizationsAsync();
                return Page();
            }
        }

        public class NewEventViewModel
        {
            [SelectItems(nameof(Organizations))]
            [DisplayName("Organization")]
            public Guid OrganizationId { get; set; }

            [Required]
            [StringLength(EventConsts.MaxTitleLength, MinimumLength = EventConsts.MinTitleLength)]
            public string Title { get; set; }

            [Required]
            public DateTime StartTime { get; set; }

            [Required]
            public DateTime EndTime { get; set; }

            [Required]
            [StringLength(EventConsts.MaxDescriptionLength, MinimumLength = EventConsts.MinDescriptionLength)]
            [TextArea]
            public string Description { get; set; }

            public bool IsOnline { get; set; }

            public int? Capacity { get; set; }
        }
    }
}
