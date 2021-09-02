using Volo.Abp.Application.Dtos;

namespace EventHub.Admin.Users
{
    public class GetUserListInput : PagedAndSortedResultRequestDto
    {
        public string Username { get; set; }
    }
}
