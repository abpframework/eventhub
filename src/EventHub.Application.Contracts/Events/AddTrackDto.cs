using System.ComponentModel.DataAnnotations;

namespace EventHub.Events;

public class AddTrackDto
{
    [Required]
    [StringLength(TrackConsts.MaxNameLength)]
    public string Name { get; set; }
}
