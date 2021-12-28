using System;
using System.Collections.Generic;
using System.Linq;
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
	[Area("eventhub")]
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
		[Route("draft-events-by-user-id")]
		public async Task<List<EventInListDto>> GetDraftEventsByCurrentUser()
		{
			return await _eventAppService.GetDraftEventsByCurrentUser();
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

		[HttpPut]
		[Route("/publish/{id}")]
		public async Task<string> PublishAsync(Guid id)
		{
			return await _eventAppService.PublishAsync(id);
		}

		[HttpPost]
		[Route("{id}/tracks")]
		public async Task AddTrackAsync(Guid id, AddTrackDto input)
		{
			await _eventAppService.AddTrackAsync(id, input);
		}

		[HttpPut]
		[Route("{id}/tracks/{trackId}")]
		public async Task UpdateTrackAsync(Guid id, Guid trackId, UpdateTrackDto input)
		{
			await _eventAppService.UpdateTrackAsync(id, trackId, input);
		}

		[HttpDelete]
		[Route("{id}/tracks/{trackId}")]
		public async Task DeleteTrackAsync(Guid id, Guid trackId)
		{
			await _eventAppService.DeleteTrackAsync(id, trackId);
		}

		[HttpPost]
		[Route("{id}/tracks/{trackId}/sessions")]
		public async Task AddSessionAsync(Guid id, Guid trackId, AddSessionDto input)
		{
			input.SpeakerUserNames = input.SpeakerUserNames.Where(x => !x.IsNullOrWhiteSpace()).ToList();
			await _eventAppService.AddSessionAsync(id, trackId, input);
		}

		[HttpPut]
		[Route("{id}/tracks/{trackId}/sessions/{sessionId}")]
		public async Task UpdateSessionAsync(Guid id, Guid trackId, Guid sessionId, UpdateSessionDto input)
		{
			input.SpeakerUserNames = input.SpeakerUserNames.Where(x => !x.IsNullOrWhiteSpace()).ToList();
			await _eventAppService.UpdateSessionAsync(id, trackId, sessionId, input);
		}

		[HttpDelete]
		[Route("{id}/tracks/{trackId}/sessions/{sessionId}")]
		public async Task DeleteSessionAsync(Guid id, Guid trackId, Guid sessionId)
		{
			await _eventAppService.DeleteSessionAsync(id, trackId, sessionId);
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
