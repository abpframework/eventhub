using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace EventHub.Events
{
    public interface IEventAppService : IApplicationService
    {
        Task<EventDto> CreateAsync(CreateEventDto input);

        Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input);

        Task<List<EventInListDto>> GetDraftEventsByUserId(Guid userId);

        Task<EventDetailDto> GetByUrlCodeAsync(string urlCode);

        Task<EventLocationDto> GetLocationAsync(Guid id);

        Task<List<CountryLookupDto>> GetCountriesLookupAsync();

        Task<bool> IsEventOwnerAsync(Guid id);

        Task UpdateAsync(Guid id, UpdateEventDto input);

        Task AddTrackAsync(Guid id, AddTractDto input);

        Task AddSessionAsync(Guid id, AddSessionDto input);

        Task<IRemoteStreamContent> GetCoverImageAsync(Guid id);
    }
}
