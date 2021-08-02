using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Organizations
{
    public class OrganizationListFilterDto : PagedAndSortedResultRequestDto
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public int? MinMemberCount { get; set; }

        public int? MaxMemberCount { get; set; }
    }
}
