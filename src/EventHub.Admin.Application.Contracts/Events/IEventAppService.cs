using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EventHub.Admin.Events
{
    public interface IEventAppService : IApplicationService
    {
        Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input);

        Task<EventDetailDto> GetAsync(Guid id);

        Task UpdateAsync(Guid id, UpdateEventDto input);
    }
}
