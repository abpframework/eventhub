using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Volo.Abp.Content;

namespace EventHub.Organizations
{
    public class CreateOrganizationDto
    {
        [Required]
        [StringLength(OrganizationConsts.MaxNameLength, MinimumLength = OrganizationConsts.MinNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(OrganizationConsts.MaxDisplayNameLength, MinimumLength = OrganizationConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(OrganizationConsts.MaxDescriptionNameLength, MinimumLength = OrganizationConsts.MinDescriptionNameLength)]
        public string Description { get; set; }
        
        [CanBeNull]
        public RemoteStreamContent ProfilePictureStreamContent { get; set; }

        [StringLength(OrganizationConsts.MaxWebsiteLength)]
        public string Website { get; set; }

        [StringLength(OrganizationConsts.MaxTwitterUsernameLength)]
        public string TwitterUsername { get; set; }

        [StringLength(OrganizationConsts.MaxGitHubUsernameLength)]
        public string GitHubUsername { get; set; }

        [StringLength(OrganizationConsts.MaxFacebookUsernameLength)]
        public string FacebookUsername { get; set; }

        [StringLength(OrganizationConsts.MaxInstagramUsernameLength)]
        public string InstagramUsername { get; set; }

        [StringLength(OrganizationConsts.MaxMediumUsernameLength)]
        public string MediumUsername { get; set; }
    }
}
