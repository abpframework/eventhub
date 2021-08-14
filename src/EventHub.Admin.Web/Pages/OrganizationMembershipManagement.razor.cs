using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.DataGrid;
using EventHub.Admin.Organizations;
using EventHub.Admin.Organizations.Memberships;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Web.Pages
{
    public partial class OrganizationMembershipManagement
    {
        [Parameter]
        public Guid? OrganizationId { get; set; }
        private string OrganizationName { get; set; }
        
        [Inject]
        protected IOrganizationAppService OrganizationAppService { get; set; }


        private IReadOnlyList<OrganizationMemberDto> OrganizationMemberList { get; set; }
        private OrganizationMemberListFilterDto Filter { get; set; }
        private int CurrentPage { get; set; }
        private int TotalCount { get; set; }
        private int PageSize { get; }

        public OrganizationMembershipManagement()
        {
            Filter = new OrganizationMemberListFilterDto();
            PageSize = LimitedResultRequestDto.DefaultMaxResultCount;

            OrganizationMemberList = new List<OrganizationMemberDto>();
        }
        
        protected override async Task OnInitializedAsync()
        {
            await GetOrganizationMembershipsAsync();

            if (OrganizationId.HasValue)
            {
                OrganizationName = (await OrganizationAppService.GetAsync(OrganizationId.Value)).Name;
            }
        }
        
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrganizationMemberDto> e)
        {
            CurrentPage = e.Page - 1;
            await GetOrganizationMembershipsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task GetOrganizationMembershipsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = CurrentPage * PageSize;
            
            if (OrganizationId.HasValue)
            {
                Filter.OrganizationId = OrganizationId!.Value;
            }
            else
            {
                Filter.OrganizationId = null;
            }

            if (!Filter.UserName.IsNullOrWhiteSpace())
            {
                Filter.UserName = Filter.UserName.Trim();
            }

            var result = await OrganizationMembershipAppService.GetListAsync(Filter);
            OrganizationMemberList = result.Items;
            TotalCount = (int) result.TotalCount;
        }
        
        private async Task OnKeyPress(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                if (OrganizationName.IsNullOrWhiteSpace())
                {
                    OrganizationId = null;
                    OrganizationName = null;
                }
                else
                {
                    var organization = await OrganizationAppService.GetByNameAsync(OrganizationName.Trim());
                    OrganizationId = organization.Id;
                    OrganizationName = organization.Name;
                }
                
                await GetOrganizationMembershipsAsync();
            }
        }
    }
}