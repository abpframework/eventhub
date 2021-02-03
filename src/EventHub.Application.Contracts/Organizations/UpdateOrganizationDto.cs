using System.ComponentModel.DataAnnotations;

namespace EventHub.Organizations
{
    public class UpdateOrganizationDto
    {
        [Required]
        [StringLength(OrganizationConsts.MaxDisplayNameLength, MinimumLength = OrganizationConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(OrganizationConsts.MaxDescriptionNameLength, MinimumLength = OrganizationConsts.MinDescriptionNameLength)]
        public string Description { get; set; }
        
        public string Website { get; set; }

        public string TwitterUsername { get; set; }

        public string GitHubUsername { get; set; }

        public string FacebookUsername { get; set; }

        public string InstagramUsername { get; set; }

        public string MediumUsername { get; set; }
    }
}