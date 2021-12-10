using System;
using Volo.Abp.Application.Dtos;

namespace EventHub.Events;

public class SpeakerDto : EntityDto<Guid>
{
    public Guid SessionId { get; set; }
        
    public Guid UserId { get; set; }

    public string UserName { get; set; }
}
