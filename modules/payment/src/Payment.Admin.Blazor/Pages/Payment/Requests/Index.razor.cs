using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Payment.Admin.Payments;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace Payment.Admin.Pages.Payment.Requests
{
    public partial class Index
    {
        [Inject] protected IPaymentRequestAdminAppService PaymentRequestAdminAppService { get; set; }
        
        protected PageToolbar Toolbar { get; } = new();

        protected TableColumnDictionary TableColumns { get; } = new();

        protected List<TableColumn> PaymentRequestTableColumns => TableColumns.Get<Index>();

        protected List<BreadcrumbItem> BreadcrumbItems { get; } = new();

        protected IReadOnlyList<PaymentRequestWithDetailsDto> Entities = Array.Empty<PaymentRequestWithDetailsDto>();

        protected virtual int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;

        protected int CurrentPage { get; set; } = 1;

        protected string CurrentSorting { get; set; }

        protected int? TotalCount { get; set; }

        protected PaymentRequestGetListInput GetListInput { get; } = new();

        protected override async Task OnInitializedAsync()
        {
            await SetBreadCrumbsAsync();
            await SetTableColumnsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private ValueTask SetBreadCrumbsAsync()
        {
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:PaymentManagement"]));
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:PaymentRequests"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetTableColumnsAsync()
        {
            PaymentRequestTableColumns
                .AddRange(new TableColumn[]
                {
                    new TableColumn
                    {
                        Title = L["DisplayName:CreationTime"],
                        Data = nameof(PaymentRequestWithDetailsDto.CreationTime)
                    },
                    new TableColumn
                    {
                        Title = L["DisplayName:Price"],
                        Data = nameof(PaymentRequestWithDetailsDto.Price)
                    },
                    new TableColumn
                    {
                        Title = L["DisplayName:Currency"],
                        Data = nameof(PaymentRequestWithDetailsDto.Currency)
                    },
                    new TableColumn
                    {
                        Title = L["DisplayName:State"],
                        Data = nameof(PaymentRequestWithDetailsDto.State)
                    },
                    new TableColumn
                    {
                        Title = L["DisplayName:ProductName"],
                        Data = nameof(PaymentRequestWithDetailsDto.ProductName)
                    },
                    new TableColumn
                    {
                        Title = L["DisplayName:FailReason"],
                        Data = nameof(PaymentRequestWithDetailsDto.FailReason)
                    }
                });

            return ValueTask.CompletedTask;
        }

        protected virtual async Task GetEntitiesAsync()
        {
            try
            {
                GetListInput.Sorting = CurrentSorting;
                GetListInput.SkipCount = (CurrentPage - 1) * PageSize;
                GetListInput.MaxResultCount = PageSize;

                var result = await PaymentRequestAdminAppService.GetListAsync(GetListInput);
                Entities = result.Items.As<IReadOnlyList<PaymentRequestWithDetailsDto>>();
                TotalCount = (int?) result.TotalCount;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        protected virtual async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PaymentRequestWithDetailsDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.None)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");

            CurrentPage = e.Page;

            await GetEntitiesAsync();
            await InvokeAsync(StateHasChanged);
        }
    }
}