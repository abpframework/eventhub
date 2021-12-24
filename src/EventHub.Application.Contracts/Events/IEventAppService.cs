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

        Task<List<EventInListDto>> GetDraftEventsByCurrentUser();

        Task<EventDetailDto> GetByUrlCodeAsync(string urlCode);

        Task<EventLocationDto> GetLocationAsync(Guid id);

        Task<List<CountryLookupDto>> GetCountriesLookupAsync();

        Task<bool> IsEventOwnerAsync(Guid id);

        Task UpdateAsync(Guid id, UpdateEventDto input);
        
        Task<string> PublishAsync(Guid id);

        Task AddTrackAsync(Guid id, AddTrackDto input);
        
        Task UpdateTrackAsync(Guid id, Guid trackId, UpdateTrackDto input);

        Task DeleteTrackAsync(Guid id, Guid trackId);

        Task AddSessionAsync(Guid id, Guid trackId, AddSessionDto input);

        Task UpdateSessionAsync(Guid id, Guid trackId, Guid sessionId, UpdateSessionDto input);
        
        Task DeleteSessionAsync(Guid id, Guid trackId, Guid sessionId);
        
        Task<IRemoteStreamContent> GetCoverImageAsync(Guid id);
    }
}
