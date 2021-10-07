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
using System.IO;
using System.Globalization;
using Volo.Abp;

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
            FillCoverImageUrl(EditingEvent.CoverImageContent);

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
            if (!EditingEvent.IsOnline && (!EditingEvent.CountryId.HasValue || string.IsNullOrWhiteSpace(EditingEvent.City)))
            {
                throw new UserFriendlyException(L["CountryAndCityRequiredForUpdateInPersonEvent"]);
            }

            await EventAppService.UpdateAsync(EditingEventId, EditingEvent);
            await GetEventsAsync();

            CoverImageUrl = string.Empty;
            EditEventModal.Hide();
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

        private void FillCoverImageUrl(byte[] content)
        {
            if (content.IsNullOrEmpty())
            {
                return;
            }

            var imageBase64Data = Convert.ToBase64String(content);
            var imageDataUrl = $"data:image/png;base64,{imageBase64Data}";
            CoverImageUrl = imageDataUrl;
        }

        private async Task OnCoverImageFileChanged(FileChangedEventArgs e)
        {
            FileEntry = e.Files.FirstOrDefault();
            if (FileEntry is null)
            {
                return;
            }

            using (var stream = new MemoryStream())
            {
                await FileEntry.WriteToStreamAsync(stream);

                stream.Seek(0, SeekOrigin.Begin);
                EditingEvent.CoverImageContent = stream.ToArray();
                FillCoverImageUrl(EditingEvent.CoverImageContent);
                await InvokeAsync(StateHasChanged);
            }
        }

        private void OnDeleteCoverImageButtonClicked()
        {
            EditingEvent.CoverImageContent = null;
            FileEntry = new FileEntry();
            CoverImageUrl = null;
        }

        private async Task FillCountriesAsync()
        {
            Countries = await EventAppService.GetCountriesLookupAsync();
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
