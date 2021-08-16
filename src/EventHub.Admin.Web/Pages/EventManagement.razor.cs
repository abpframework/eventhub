using EventHub.Admin.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.Application.Dtos;
using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Components.Web;

namespace EventHub.Admin.Web.Pages
{
    public partial class EventManagement
    {
        private IReadOnlyList<EventInListDto> EventList { get; set; }
        private EventListFilterDto Filter { get; set; }
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private int PageSize { get; }
        private Guid EditingEventId { get; set; }
        private EventDetailDto Event { get; set; }
        private UpdateEventDto EditingEvent { get; set; }
        private Modal EditEventModal { get; set; }
        private string SelectedTabInEditModal { get; set; } 

        public EventManagement()
        {
            Filter = new EventListFilterDto();
            PageSize = LimitedResultRequestDto.DefaultMaxResultCount;
            SelectedTabInEditModal = EventEditTabs.EventInfo.ToString();
        }

        protected override async Task OnInitializedAsync()
        {
            await GetEventsAsync();
        }

        private async Task GetEventsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = CurrentPage * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await EventAppService.GetListAsync(Filter);
            EventList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EventInListDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.Direction != SortDirection.None)
                .Select(c => c.Field + (c.Direction == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page - 1;

            await GetEventsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenEditEventModal(EventInListDto input)
        {
            EditingEventId = input.Id;
            Event = await EventAppService.GetAsync(EditingEventId);

            EditingEvent = ObjectMapper.Map<EventDetailDto, UpdateEventDto>(Event);
            EditEventModal.Show();
        }

        private void OnEditModalClosing(CancelEventArgs e)
        {
            SelectedTabInEditModal = EventEditTabs.EventInfo.ToString();
        }

        private void OnSelectedTabChangedInEditModal(string name)
        {
            SelectedTabInEditModal = name;
        }

        private async Task UpdateEventAsync()
        {
            await EventAppService.UpdateAsync(EditingEventId, EditingEvent);
            await GetEventsAsync();
            EditEventModal.Hide();
        }

        private async Task OnKeyPress(KeyboardEventArgs e)
        {
            if (e.Code is "Enter" or "NumpadEnter")
            {
                await GetEventsAsync();
            }
        }

        private async Task OnSelectedDateChanged(DateTime? changedDate)
        {
            Filter.StartTime = changedDate;
            await GetEventsAsync();
        }
    }

    public enum EventEditTabs : byte
    {
        EventInfo,
        Timing,
        CoverImage
    }
}
