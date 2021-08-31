using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Volo.Abp.Content;

namespace EventHub.Events
{
    public class CreateEventDto
    {
        [Required]
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
        
        [CanBeNull] 
        public IRemoteStreamContent CoverImageStreamContent { get; set; }

        public bool IsOnline { get; set; }
        
        [CanBeNull]
        [StringLength(EventConsts.MaxOnlineLinkLength, MinimumLength = EventConsts.MinOnlineLinkLength)]
        public string OnlineLink { get; set; }
        
        public Guid? CountryId { get; set; }
        
        [CanBeNull]
        [StringLength(EventConsts.MaxCityLength, MinimumLength = EventConsts.MinCityLength)]
        public string City { get; set; }
        
        [CanBeNull]
        [StringLength(EventConsts.MaxLanguageLength, MinimumLength = EventConsts.MinLanguageLength)]
        public string Language { get; set; }

        [Range(1, int.MaxValue)]
        public int? Capacity { get; set; }
    }}
