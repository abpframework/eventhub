using System;
using System.Collections.Generic;
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

        Task<EventLocationDto> GetLocationAsync(Guid id);

        Task<List<CountryLookupDto>> GetCountriesLookupAsync();

        Task<bool> IsEventOwnerAsync(Guid id);

        Task UpdateAsync(Guid id, UpdateEventDto input);

        Task AddSessionAsync(Guid id, AddSessionDto input);

        Task<byte[]> GetCoverImageAsync(Guid id);
    }

    public class AddSessionDto
    {
        public Guid TrackId { get; set; }
        
        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Description { get; set; }

        public string Language { get; set; }
    }
}
