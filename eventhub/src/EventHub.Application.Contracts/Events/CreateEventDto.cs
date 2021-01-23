using System;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Events
{
    public class CreateEventDto
    {
        public Guid OrganizationId { get; set; }

        [Required]
        [StringLength(EventConsts.MaxTitleLength, MinimumLength = EventConsts.MinTitleLength)]
        public string Title { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [StringLength(EventConsts.MaxDescriptionLength, MinimumLength = EventConsts.MinDescriptionLength)]
        public string Description { get; set; }

        public bool IsOnline { get; set; }

        public int? Capacity { get; set; }
    }}
