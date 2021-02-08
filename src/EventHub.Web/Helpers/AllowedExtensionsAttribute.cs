using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace EventHub.Web.Helpers
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;

        public AllowedExtensionsAttribute(string[] allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            
            var file = value as IFormFile;
            var extension = Path.GetExtension(file.FileName);
            
            if (file.Length > 0 && file != null)
            {
                if (!_allowedExtensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult("This file extension is not allowed. Allowed extensions: png, jpg, jpeg");
                }
            }

            return ValidationResult.Success;
        }
    }
}