using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace EventHub.Events;

public class SessionDto : EntityDto<Guid>
{
    public Guid TrackId { get; set; }
    
    public string Title { get; set; }

    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public string Description { get; set; }

    public string Language { get; set; }
    
    public List<SpeakerDto> Speakers { get; set; }

    public SessionDto()
    {
        Speakers = new List<SpeakerDto>();
    }
}
