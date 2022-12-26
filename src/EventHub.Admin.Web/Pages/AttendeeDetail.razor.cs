using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using EventHub.Admin.Events.Registrations;
using EventHub.Admin.Permissions;
using EventHub.Admin.Web.Components.UserPicker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Web.Pages
{
    public partial class AttendeeDetail
    {
        [Parameter]
        public Guid EventId { get; set; }
        private IReadOnlyList<EventAttendeeDto> AttendeeList { get; set; }
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private int PageSize { get; }
        private bool CanAddAttendee { get; set; }
        private GetEventRegistrationListInput Filter { get; set; }

        public UserPicker UserPickerModalRef { get; set; }
        private List<Guid> SelectedUserIds { get; set; }
        public AttendeeDetail()
        {
            PageSize = LimitedResultRequestDto.DefaultMaxResultCount;
            Filter = new GetEventRegistrationListInput();
            SelectedUserIds = new List<Guid>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetAttendeesAsync();
        }

        private async Task GetAttendeesAsync()
        {
            Filter.EventId = EventId;
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = CurrentPage * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await EventRegistrationAppService.GetAttendeesAsync(Filter);

            AttendeeList = result.Items;
            TotalCount = (int)result.TotalCount;

            SelectedUserIds = await EventRegistrationAppService.GetAllAttendeeIdsAsync(EventId);
        }

        private async Task SetPermissionsAsync()
        {
            CanAddAttendee = await AuthorizationService.IsGrantedAsync(EventHubPermissions.Events.Registrations.AddAttendee);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EventAttendeeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page - 1;

            await GetAttendeesAsync();
            await InvokeAsync(StateHasChanged);
        }
        private async Task RemoveAttendeeAsync(EventAttendeeDto attendee)
        {
            await EventRegistrationAppService.UnRegisterAttendeeAsync(EventId, attendee.UserId);
            await GetAttendeesAsync();
        }

        private async Task AddSelectedUsersToEventAsync()
        {
            try
            {
                var selectedUserIds = UserPickerModalRef
                    .SelectAllUsers
                    .Where(x => x.Value)
                    .Select(x => x.Key)
                    .ToList();

                var removedAttendees = SelectedUserIds.Except(selectedUserIds).ToList();
                foreach (var attendeeId in removedAttendees)
                {
                    await EventRegistrationAppService.UnRegisterAttendeeAsync(EventId, attendeeId);
                }

                await EventRegistrationAppService.RegisterUsersAsync(EventId, selectedUserIds);
                await UserPickerModalRef.CloseUserPickerModalAsync();

                await GetAttendeesAsync();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task OpenUserPickerModal()
        {
            await UserPickerModalRef.OpenUserPickerModalAsync();
        }
    }
}
