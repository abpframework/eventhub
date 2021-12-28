using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Events;

public class UpdateSessionDto
{
    [Required]
    [StringLength(SessionConsts.MaxTitleLength, MinimumLength = SessionConsts.MinTitleLength)]
    public string Title { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime EndTime { get; set; }

    [Required]
    [StringLength(SessionConsts.MaxDescriptionLength, MinimumLength = SessionConsts.MinDescriptionLength)]
    public string Description { get; set; }

    public string Language { get; set; }
    
    public List<string> SpeakerUserNames { get; set; }

    public UpdateSessionDto()
    {
        SpeakerUserNames = new List<string>();
    }
}
