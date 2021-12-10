using System.ComponentModel.DataAnnotations;

namespace EventHub.Events;

public class UpdateTrackDto
{
    [Required]
    [StringLength(TrackConsts.MaxNameLength)]
    public string Name { get; set; }
}
