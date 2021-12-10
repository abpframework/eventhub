using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Events;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.VirtualFileSystem;

namespace EventHub.Controllers.Events
{
	[RemoteService(Name = EventHubRemoteServiceConsts.RemoteServiceName)]
	[Area("eventhubm")]
	[ControllerName("Event")]
	[Route("api/eventhub/event")]
	public class EventController : EventHubController, IEventAppService
	{
		private readonly IEventAppService _eventAppService;
		private readonly IVirtualFileProvider _virtualFileProvider;

		public EventController(
			IEventAppService eventAppService,
			IVirtualFileProvider virtualFileProvider)
		{
			_eventAppService = eventAppService;
			_virtualFileProvider = virtualFileProvider;
		}

		[HttpPost]
		public async Task<EventDto> CreateAsync([FromForm] CreateEventDto input)
		{
			return await _eventAppService.CreateAsync(input);
		}

		[HttpGet]
		public async Task<PagedResultDto<EventInListDto>> GetListAsync(EventListFilterDto input)
		{
			return await _eventAppService.GetListAsync(input);
		}

		[HttpGet]
		[Route("draft-events-by-user-id/{userId}")]
		public async Task<List<EventInListDto>> GetDraftEventsByUserId(Guid userId)
		{
			return await _eventAppService.GetDraftEventsByUserId(userId);
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
		public async Task UpdateAsync(Guid id, [FromForm] UpdateEventDto input)
		{
			await _eventAppService.UpdateAsync(id, input);
		}

		[HttpPost]
		[Route("{id}/tracks")]
		public async Task AddTrackAsync(Guid id, AddTractDto input)
		{
			await _eventAppService.AddTrackAsync(id, input);
		}

		[HttpPost]
		[Route("{id}/tracks/{trackId}")]
		public async Task DeleteTrackAsync(Guid id, Guid trackId)
		{
			await _eventAppService.DeleteTrackAsync(id, trackId);
		}

		[HttpPost]
		[Route("{id}/sessions")]
		public async Task AddSessionAsync(Guid id, AddSessionDto input)
		{
			await _eventAppService.AddSessionAsync(id, input);
		}

		[HttpGet]
		[Route("cover-image/{id}")]
		public async Task<IRemoteStreamContent> GetCoverImageAsync(Guid id)
		{
			var remoteStreamContent = await _eventAppService.GetCoverImageAsync(id);

			if (remoteStreamContent is null)
			{
				var stream = _virtualFileProvider.GetFileInfo("/Images/eh-event.png").CreateReadStream();
				remoteStreamContent = new RemoteStreamContent(stream);
				await stream.FlushAsync();
			}

			Response.Headers.Add("Accept-Ranges", "bytes");
			Response.ContentType = remoteStreamContent.ContentType;

			return remoteStreamContent;
		}
	}
}
