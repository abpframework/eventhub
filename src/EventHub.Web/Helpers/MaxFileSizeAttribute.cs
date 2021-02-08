using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EventHub.Web.Helpers
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var file = value as IFormFile;

            if (file.Length > 0)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult("Maximum file size: " + _maxFileSize);
                }
            }

            return ValidationResult.Success;
        }
    }
}