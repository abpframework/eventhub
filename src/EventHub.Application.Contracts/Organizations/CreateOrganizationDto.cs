using System.ComponentModel.DataAnnotations;

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
    }
}
