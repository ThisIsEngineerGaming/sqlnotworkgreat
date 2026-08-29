using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace mvc.Models.Validation
{
    // Custom validation attribute.
    // The field is optional (the user may upload a file instead of a link),
    // but if a value is provided it must be a well-formed absolute URL
    // using the http or https scheme.
    public class ValidImageUrlAttribute : ValidationAttribute, IClientModelValidator
    {
        public ValidImageUrlAttribute()
        {
            ErrorMessage = "The {0} field must be a valid link (http:// or https://).";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string url && !string.IsNullOrWhiteSpace(url))
            {
                bool isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

                if (!isValidUrl)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            // Empty value is allowed — the photo may be uploaded as a file instead.
            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            string message = FormatErrorMessage(context.ModelMetadata.GetDisplayName());

            if (!context.Attributes.ContainsKey("data-val"))
            {
                context.Attributes.Add("data-val", "true");
            }

            if (!context.Attributes.ContainsKey("data-val-validimageurl"))
            {
                context.Attributes.Add("data-val-validimageurl", message);
            }
        }
    }
}
