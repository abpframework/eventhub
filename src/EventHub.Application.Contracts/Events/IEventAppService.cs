using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Events
{
    public interface IEventAppService : IApplicationService
    {
        Task<EventDto> CreateAsync(CreateEventDto input);

        Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input);

        Task<EventDetailDto> GetByUrlCodeAsync(string urlCode);
    }
}
