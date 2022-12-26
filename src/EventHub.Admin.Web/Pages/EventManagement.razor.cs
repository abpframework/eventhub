using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using EventHub.Admin.Events;
using Microsoft.AspNetCore.Components.Web;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

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
        private string CoverImageUrl { get; set; }
        private IFileEntry FileEntry { get; set; }
        private List<CountryLookupDto> Countries { get; set; }
        private List<Language> Languages { get; set; }
        private bool DisabledCoverImageButton { get; set; }

        public EventManagement()
        {
            Filter = new EventListFilterDto();
            PageSize = LimitedResultRequestDto.DefaultMaxResultCount;
            SelectedTabInEditModal = EventEditTabs.EventInfo.ToString();
            EditingEvent = new UpdateEventDto();
            Countries = new List<CountryLookupDto>();
            Languages = new List<Language>();
        }

        protected override async Task OnInitializedAsync()
        {
            await GetEventsAsync();
            await FillCountriesAsync();
            FillLanguages();
        }

        private void FillLanguages()
        {
            var result = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                .DistinctBy(x => x.EnglishName)
                .OrderBy(x => x.EnglishName)
                .ToList();
            result.Remove(result.Single(x => x.TwoLetterISOLanguageName == "iv")); // Invariant Language

            Languages = result.Select(cultureInfo => new Language
            {
                Value = cultureInfo.TwoLetterISOLanguageName,
                Text = cultureInfo.EnglishName
            }).ToList();
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
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page - 1;

            await GetEventsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenEditEventModalAsync(EventInListDto input)
        {
            EditingEventId = input.Id;
            Event = await EventAppService.GetAsync(EditingEventId);
            EditingEvent = ObjectMapper.Map<EventDetailDto, UpdateEventDto>(Event);
            
            FileEntry = new FileEntry();
            await SetCoverImageUrlAsync();

            await EditEventModal.Show();
        }

        private Task OnEditModalClosing(CancelEventArgs e)
        {
            SelectedTabInEditModal = EventEditTabs.EventInfo.ToString();
            
            return Task.CompletedTask;
        }

        private void OnSelectedTabChangedInEditModal(string name)
        {
            SelectedTabInEditModal = name;
        }

        private async Task UpdateEventAsync()
        {
            if (!EditingEvent.IsOnline && (!EditingEvent.CountryId.HasValue || string.IsNullOrWhiteSpace(EditingEvent.City)))
            {
                throw new UserFriendlyException(L["CountryAndCityRequiredForUpdateInPersonEvent"]);
            }

            await EventAppService.UpdateAsync(EditingEventId, EditingEvent);
            await GetEventsAsync();

            CoverImageUrl = string.Empty;
            DisabledCoverImageButton = false;
            await EditEventModal.Hide();
        }

        private async Task OnKeyPress(KeyboardEventArgs e)
        {
            if (e.Code is "Enter" or "NumpadEnter")
            {
                await GetEventsAsync();
            }
        }

        private async Task OnSelectedDateChangedForMinStartTime(DateTime? minStartTime)
        {
            Filter.MinStartTime = minStartTime;
            await GetEventsAsync();
        }

        private async Task OnSelectedDateChangedForMaxStartTime(DateTime? maxStartTime)
        {
            Filter.MaxStartTime = maxStartTime;
            await GetEventsAsync();
        }

        private async Task OnCoverImageFileChanged(FileChangedEventArgs e)
        {
            FileEntry = e.Files.FirstOrDefault();
            if (FileEntry is null)
            {
                return;
            }

            var stream = new MemoryStream();
            await FileEntry.WriteToStreamAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            EditingEvent.CoverImageStreamContent = new RemoteStreamContent(stream);

            void SetCoverImageUrl(string contentType, byte[] content)
            {
                if (content.IsNullOrEmpty())
                {
                    return;
                }

                contentType = string.IsNullOrWhiteSpace(contentType) ? "image/png" : contentType;
                CoverImageUrl = $"data:{contentType};base64,{Convert.ToBase64String(content)}";
                DisabledCoverImageButton = false;
            }
            
            SetCoverImageUrl(EditingEvent.CoverImageStreamContent.ContentType, stream.ToArray());
            
            await stream.FlushAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OnDeleteCoverImageButtonClicked()
        {
            EditingEvent.CoverImageStreamContent = null;
            FileEntry = new FileEntry();
            CoverImageUrl = null;
            DisabledCoverImageButton = true;
        }

        private async Task FillCountriesAsync()
        {
            Countries = await EventAppService.GetCountriesLookupAsync();
        }
        
        private async Task SetCoverImageUrlAsync()
        {
            var imageRemoteStreamContent = await EventAppService.GetCoverImageAsync(id: EditingEventId);

            if (imageRemoteStreamContent.ContentLength <= 0)
            {
                CoverImageUrl = "/assets/eh-event.png";
                DisabledCoverImageButton = true;
                return;
            }
            CoverImageUrl = UrlOptions.Value.AdminApi.EnsureEndsWith('/') + "api/eventhub/admin/event/cover-image/" + EditingEventId;
        }

        private enum EventEditTabs : byte
        {
            EventInfo,
            Timing,
            CoverImage
        }

        private class Language
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }
    }
}
