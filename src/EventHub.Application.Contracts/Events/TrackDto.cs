using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Events;

public class TrackDto : EntityDto<Guid>
{
    public string Name { get; set; }
}
