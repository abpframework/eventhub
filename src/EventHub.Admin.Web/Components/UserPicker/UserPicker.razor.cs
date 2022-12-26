using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using EventHub.Admin.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Web.Components.UserPicker
{
    public partial class UserPicker
    {
        public Modal UserPickerModal { get; set; }

        [Parameter]
        public EventCallback SaveFormAsync { get; set; }

        [Parameter] 
        public List<Guid> SelectedUserIds { get; set; } = new List<Guid>();

        private GetUserListInput Filter { get; set; }

        private IReadOnlyList<UserDto> UserList { get; set; }
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private int PageSize { get; }
        
        public Dictionary<Guid, bool> SelectAllUsers = new();
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

        public UserPicker()
        {
            Filter = new GetUserListInput();
            PageSize = LimitedResultRequestDto.DefaultMaxResultCount;
        }

        protected override async Task OnInitializedAsync()
        {
            await GetUsersAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            await GetUsersAsync();
        }

        private async Task GetUsersAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = CurrentPage * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await UserAppService.GetListAsync(Filter);
            UserList = result.Items;
            TotalCount = (int)result.TotalCount;

            FillSelectedUserList();
        }

        private void FillSelectedUserList()
        {
            SelectAllUsers = new Dictionary<Guid, bool>();

            foreach (var user in UserList)
            {
                SelectAllUsers.Add(user.Id, SelectedUserIds.Contains(user.Id));
            }
        }

        private async Task OnKeyPressed(KeyboardEventArgs e)
        {
            if (e.Code is "Enter" or "NumpadEnter")
            {
                await GetUsersAsync();
            }
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page - 1;

            await GetUsersAsync();
            await InvokeAsync(StateHasChanged);
        }

        public Task OpenUserPickerModalAsync()
        {
            UserPickerModal.Show();

            return Task.CompletedTask;
        }

        public Task CloseUserPickerModalAsync()
        {
            UserPickerModal.Hide();

            return Task.CompletedTask;
        }

        private async Task SaveUserPickerFormAsync()
        {
            await SaveFormAsync.InvokeAsync();
        }

        private Task ClosingUserPickerModal(ModalClosingEventArgs eventArgs)
        {
            eventArgs.Cancel = eventArgs.CloseReason == CloseReason.FocusLostClosing;
            
            return Task.CompletedTask;
        }
    }
}
