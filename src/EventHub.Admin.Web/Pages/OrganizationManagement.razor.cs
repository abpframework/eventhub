using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using EventHub.Admin.Organizations;
using Microsoft.AspNetCore.Components.Web;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace EventHub.Admin.Web.Pages
{
    public partial class OrganizationManagement
    {
        private IReadOnlyList<OrganizationInListDto> OrganizationList { get; set; }
        private OrganizationListFilterDto Filter { get; set; }
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private int PageSize { get; }

        private OrganizationProfileDto Organization { get; set; }
        private string ProfileImageUrl { get; set; }

        private Guid EditingOrganizationId { get; set; }
        private UpdateOrganizationDto EditingOrganization { get; set; }
        private Modal EditOrganizationModal { get; set; }
        private IFileEntry FileEntry { get; set; }
        private bool IsLoadingProfileImage { get; set; }
        private string SelectedTabInEditModal { get; set; }
        private bool DisabledProfileImageButton { get; set; }

        public OrganizationManagement()
        {
            Filter = new OrganizationListFilterDto();
            PageSize = LimitedResultRequestDto.DefaultMaxResultCount;

            OrganizationList = new List<OrganizationInListDto>();
            EditingOrganization = new UpdateOrganizationDto();
            IsLoadingProfileImage = false;
            SelectedTabInEditModal = TabContentInEditModal.OrganizationProfile.ToString();
        }

        protected override async Task OnInitializedAsync()
        {
            await GetOrganizationsAsync();
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrganizationInListDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page - 1;
            await GetOrganizationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task GetOrganizationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = CurrentPage * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await OrganizationAppService.GetListAsync(Filter);
            OrganizationList = result.Items;
            TotalCount = (int) result.TotalCount;
        }

        private async Task OpenEditOrganizationModalAsync(OrganizationInListDto input)
        {
            EditingOrganizationId = input.Id;
            Organization = await OrganizationAppService.GetAsync(EditingOrganizationId);

            FileEntry = new FileEntry();
            await SetProfileImageUrlAsync();

            EditingOrganization = ObjectMapper.Map<OrganizationProfileDto, UpdateOrganizationDto>(Organization);
            await EditOrganizationModal.Show();
        }

        private async Task UpdateOrganizationAsync()
        {
            await OrganizationAppService.UpdateAsync(EditingOrganizationId, EditingOrganization);
            await GetOrganizationsAsync();
            await EditOrganizationModal.Hide();

            ProfileImageUrl = null;
            DisabledProfileImageButton = false;
        }

        private void OnDeleteCoverImageButtonClicked()
        {
            EditingOrganization.ProfilePictureStreamContent = null;
            FileEntry = new FileEntry();
            ProfileImageUrl = null;
            DisabledProfileImageButton = true;
            IsLoadingProfileImage = false;
        }

        private void CloseUpdateOrganizationModal()
        {
            DisabledProfileImageButton = false;
            EditOrganizationModal.Hide();
        }
        
        private async Task DeleteOrganizationAsync(OrganizationInListDto organization)
        {
            await OrganizationAppService.DeleteAsync(organization.Id);
            await GetOrganizationsAsync();
        }

        private async Task OnProfileImageFileChanged(FileChangedEventArgs e)
        {
            FileEntry = e.Files.FirstOrDefault();
            if (FileEntry is null)
            {
                return;
            }

            IsLoadingProfileImage = true;

            var stream = new MemoryStream();
            await FileEntry.WriteToStreamAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            EditingOrganization.ProfilePictureStreamContent = new RemoteStreamContent(stream);

            void SetProfileImageUrl(string contentType, byte[] content)
            {
                if (content != null)
                {
                    contentType = string.IsNullOrWhiteSpace(contentType) ? "image/png" : contentType;
                    var imageDataUrl = $"data:{contentType};base64,{Convert.ToBase64String(content)}";
                    ProfileImageUrl = imageDataUrl;
                    DisabledProfileImageButton = false;
                }
            }
            
            SetProfileImageUrl(EditingOrganization.ProfilePictureStreamContent.ContentType, stream.ToArray());

            await stream.FlushAsync();
            await InvokeAsync(StateHasChanged);
        }

        private void OnProgressedForProfileImage(FileProgressedEventArgs e)
        {
            if (e.Percentage == 100D)
            {
                IsLoadingProfileImage = false;
            }
        }

        private void OnSelectedTabChangedInEditModal(string name)
        {
            SelectedTabInEditModal = name;
        }

        private Task OnEditModalClosing(CancelEventArgs e)
        {
            IsLoadingProfileImage = false;
            SelectedTabInEditModal = TabContentInEditModal.OrganizationProfile.ToString();
            
            
            return Task.CompletedTask;
        }

        private async Task OnKeyPress(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await GetOrganizationsAsync();
            }
        }
        
        private async Task SetProfileImageUrlAsync()
        {
            var imageRemoteStreamContent = await OrganizationAppService.GetCoverImageAsync(id: EditingOrganizationId);
            if (imageRemoteStreamContent.ContentLength <= 0)
            {
                ProfileImageUrl = "assets/eh-organization.png";
                DisabledProfileImageButton = true;
                return;
            }
            ProfileImageUrl = UrlOptions.Value.AdminApi.EnsureEndsWith('/') + "api/eventhub/admin/organization/cover-image/" + EditingOrganizationId;
        }
    }

    public enum TabContentInEditModal : byte
    {
        OrganizationProfile = 0,
        ProfileImage
    }
}
