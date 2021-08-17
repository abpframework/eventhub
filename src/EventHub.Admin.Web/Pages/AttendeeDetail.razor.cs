using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using EventHub.Admin.Events.Registrations;
using System.Collections.Generic;
using System.Linq;
using Blazorise;
using Volo.Abp.Application.Dtos;
using Blazorise.DataGrid;
using EventHub.Admin.Permissions;
using EventHub.Admin.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;

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
        private Modal AddAttendeeModal { get; set; }
        private GetUnregisteredUserEventInput Filter { get; set; }
        private IReadOnlyList<UserDto> UserList { get; set; }
        private int UserListTotalCount { get; set; }

        private Dictionary<Guid, bool> SelectAllUsers = new();

        private bool AllUserSelected
        {
            get => SelectAllUsers.All(x => x.Value);
            set
            {
                foreach (var key in SelectAllUsers.Keys)
                {
                    SelectAllUsers[key] = value;
                }
            }
        }

        public AttendeeDetail()
        {
            PageSize = LimitedResultRequestDto.DefaultMaxResultCount;
            Filter = new GetUnregisteredUserEventInput();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetAttendeesAsync();
        }

        private async Task GetAttendeesAsync()
        {
            var result = await EventRegistrationAppService.GetAttendeesAsync(EventId);

            AttendeeList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        private async Task SetPermissionsAsync()
        {
            CanAddAttendee = await AuthorizationService.IsGrantedAsync(EventHubPermissions.Events.Registrations.AddAttendee);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EventAttendeeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.Direction != SortDirection.None)
                .Select(c => c.Field + (c.Direction == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page - 1;

            await GetAttendeesAsync();
            await InvokeAsync(StateHasChanged);
        }
        private async Task RemoveAttendeeAsync(EventAttendeeDto attendee)
        {
            await EventRegistrationAppService.RemoveAttendeeAsync(EventId, attendee.Id);
            await GetAttendeesAsync();
        }

        private async Task OpenAddAttendeeModal()
        {
            AddAttendeeModal.Show();
            await GetUsersAsync();
        }

        private async Task GetUsersAsync()
        {
            Filter.EventId = EventId;
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = CurrentPage * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await UserAppService.GetUnregisteredUsersOfEventAsync(Filter);
            UserList = result.Items;
            UserListTotalCount = (int)result.TotalCount;

            FillSelectedUserList();
        }

        private void FillSelectedUserList()
        {
            SelectAllUsers = new Dictionary<Guid, bool>();

            foreach (var user in UserList)
            {
                SelectAllUsers.Add(user.Id, false);
            }
        }

        private async Task OnKeyPressed(KeyboardEventArgs e)
        {
            if (e.Code is "Enter" or "NumpadEnter")
            {
                await GetUsersAsync();
            }
        }

        private async Task OnDataGridReadForUsersAsync(DataGridReadDataEventArgs<UserDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.Direction != SortDirection.None)
                .Select(c => c.Field + (c.Direction == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page - 1;

            await GetUsersAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task AddSelectedUsersToEventAsync()
        {
            try
            {
                var selectedUserIds= SelectAllUsers
                    .Where(x => x.Value)
                    .Select(x => x.Key)
                    .ToList();

                await EventRegistrationAppService.RegisterUsersAsync(EventId, selectedUserIds);
                await CloseAddAttendeeModalAsync();

                await GetAttendeesAsync();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void ClosingAddAttendeeModal(ModalClosingEventArgs eventArgs)
        {
            eventArgs.Cancel = eventArgs.CloseReason == CloseReason.FocusLostClosing;
        }

        private Task CloseAddAttendeeModalAsync()
        {
            AddAttendeeModal.Hide();
            return Task.CompletedTask;
        }
    }
}
