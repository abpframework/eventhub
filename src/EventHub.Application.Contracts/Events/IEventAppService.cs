using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.SettingManagement;

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

        Task<IRemoteStreamContent> GetCoverImageAsync(Guid id);
    }

    public class AddSessionDto
    {
        public Guid TrackId { get; set; }
        [Required]
        [StringLength(SessionConsts.MaxTitleLength,
            MinimumLength = SessionConsts.MinTitleLength)]
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [Required]
        [StringLength(SessionConsts.MaxDescriptionLength,
            MinimumLength = SessionConsts.MinDescriptionLength)]
        public string Description { get; set; }
        public string Language { get; set; }
    }
}
