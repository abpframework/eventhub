using System;
using System.ComponentModel.DataAnnotations;
using EventHub.Events;

namespace EventHub.Admin.Events
{
    public class UpdateEventDto
    {
        [Required]
        [StringLength(EventConsts.MaxTitleLength, MinimumLength = EventConsts.MinTitleLength)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }
    }
}
