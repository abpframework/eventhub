using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Admin.Events;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace EventHub.Admin.Controllers.Events
{
    [RemoteService(Name = EventHubAdminRemoteServiceConsts.RemoteServiceName)]
    [Controller]
    [Area("eventhub-admin")]
    [ControllerName("Event")]
    [Route("/api/eventhub/admin/event")]
    public class EventController : AbpController, IEventAppService
    {
        private readonly IEventAppService _eventAppService;

        public EventController(IEventAppService eventAppService)
        {
            _eventAppService = eventAppService;
        }

        [HttpGet("{id}")]
        public Task<EventDetailDto> GetAsync(Guid id)
        {
            return _eventAppService.GetAsync(id);
        }

        [HttpGet("countries")]
        public Task<List<CountryLookupDto>> GetCountriesLookupAsync()
        {
            return _eventAppService.GetCountriesLookupAsync();
        }

        [HttpGet("cover-image/{id}")]
        public Task<byte[]> GetCoverImageAsync(Guid id)
        {
            return _eventAppService.GetCoverImageAsync(id);
        }

        [HttpGet]
        public Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input)
        {
            return _eventAppService.GetListAsync(input);
        }

        [HttpPut]
        public Task UpdateAsync(Guid id, UpdateEventDto input)
        {
            return _eventAppService.UpdateAsync(id, input);
        }
    }
}
