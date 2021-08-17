using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Controllers.Events
{
    [RemoteService(Name = EventHubRemoteServiceConsts.RemoteServiceName)]
    [Area("eventhub")]
    [ControllerName("Event")]
    [Route("api/eventhub/event")]
    public class EventController : AbpController, IEventAppService
    {
        private readonly IEventAppService _eventAppService;

        public EventController(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        [HttpPost]
        public async Task<EventDto> CreateAsync(CreateEventDto input)
        {
            return await _eventAppService.CreateAsync(input);
        }

        [HttpGet]
        public async Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input)
        {
            return await _eventAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("by-url-code/{urlCode}")]
        public async Task<EventDetailDto> GetByUrlCodeAsync(string urlCode)
        {
            return await _eventAppService.GetByUrlCodeAsync(urlCode);
        }

        [HttpGet]
        [Route("location/{id}")]
        public async Task<EventLocationDto> GetLocationAsync(Guid id)
        {
            return await _eventAppService.GetLocationAsync(id);
        }

        [HttpGet]
        [Route("lookup/countries")]
        public async Task<List<CountryLookupDto>> GetCountriesLookupAsync()
        {
            return await _eventAppService.GetCountriesLookupAsync();
        }

        [HttpGet]
        [Route("is-event-owner/{id}")]
        public async Task<bool> IsEventOwnerAsync(Guid id)
        {
            return await _eventAppService.IsEventOwnerAsync(id);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task UpdateAsync(Guid id, UpdateEventDto input)
        {
            await _eventAppService.UpdateAsync(id, input);
        }

        [HttpPost]
        [Route("{id}/sessions")]
        public async Task AddSessionAsync(Guid id, AddSessionDto input)
        {
            await _eventAppService.AddSessionAsync(id, input);
        }

        [HttpGet]
        [Route("cover-image/{id}")]
        public async Task<byte[]> GetCoverImageAsync(Guid id)
        {
            return await _eventAppService.GetCoverImageAsync(id);
        }
    }
}