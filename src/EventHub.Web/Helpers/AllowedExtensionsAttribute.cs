#nullable enable
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
            var file = value as IFormFile;
            
            if (file is { Length: > 0 })
            {
                var extension = Path.GetExtension(file.FileName);

                if (!_allowedExtensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult("This file extension is not allowed. Allowed extensions: " + string.Join(",", _allowedExtensions));
                }
            }

            return ValidationResult.Success;
        }
    }
}