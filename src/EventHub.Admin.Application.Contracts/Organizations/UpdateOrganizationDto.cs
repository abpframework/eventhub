using System.ComponentModel.DataAnnotations;
using EventHub.Organizations;
using JetBrains.Annotations;
using Volo.Abp.Content;

namespace EventHub.Admin.Organizations
{
    public class UpdateOrganizationDto
    {
        [Required]
        [StringLength(OrganizationConsts.MaxDisplayNameLength, MinimumLength = OrganizationConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(OrganizationConsts.MaxDescriptionNameLength, MinimumLength = OrganizationConsts.MinDescriptionNameLength)]
        public string Description { get; set; }
        
        [CanBeNull]
        public IRemoteStreamContent ProfilePictureStreamContent { get; set; }

        [CanBeNull]
        [StringLength(OrganizationConsts.MaxWebsiteLength)]
        public string Website { get; set; }
        
        [CanBeNull]
        [StringLength(OrganizationConsts.MaxTwitterUsernameLength)]
        public string TwitterUsername { get; set; }
        
        [CanBeNull]
        [StringLength(OrganizationConsts.MaxGitHubUsernameLength)]
        public string GitHubUsername { get; set; }
        
        [CanBeNull]
        [StringLength(OrganizationConsts.MaxFacebookUsernameLength)]
        public string FacebookUsername { get; set; }
        
        [CanBeNull]
        [StringLength(OrganizationConsts.MaxInstagramUsernameLength)]
        public string InstagramUsername { get; set; }
        
        [CanBeNull]
        [StringLength(OrganizationConsts.MaxMediumUsernameLength)]
        public string MediumUsername { get; set; }
    }
}