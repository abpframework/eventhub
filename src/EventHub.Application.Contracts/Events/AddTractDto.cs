using System.ComponentModel.DataAnnotations;

namespace EventHub.Events;

public class AddTractDto
{
    [Required]
    [StringLength(TrackConsts.MaxNameLength)]
    public string Name { get; set; }
}
