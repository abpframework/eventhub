using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace EventHub.Events;

public class TrackDto : EntityDto<Guid>
{
    public string Name { get; set; }

    public List<SessionDto> Sessions { get; set; }

    public TrackDto()
    {
        Sessions = new List<SessionDto>();
    }
}
