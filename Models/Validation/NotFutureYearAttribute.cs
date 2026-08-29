using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace mvc.Models.Validation
{
    // Custom validation attribute.
    // Checks that the film's release year falls within the range from the
    // invention of cinema (1888) up to the current year (not in the future).
    // Implements IClientModelValidator so the same check also runs on the
    // client side (in the browser) via jQuery Unobtrusive Validation.
    public class NotFutureYearAttribute : ValidationAttribute, IClientModelValidator
    {
        public const int MinYear = 1888; // the year the first film in history was made

        public NotFutureYearAttribute()
        {
            ErrorMessage = "The {0} field must be a year between " + MinYear + " and " + DateTime.Now.Year + ".";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int year)
            {
                int maxYear = DateTime.Now.Year;
                if (year < MinYear || year > maxYear)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            int maxYear = DateTime.Now.Year;
            string message = FormatErrorMessage(context.ModelMetadata.GetDisplayName());

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-notfutureyear", message);
            MergeAttribute(context.Attributes, "data-val-notfutureyear-min", MinYear.ToString());
            MergeAttribute(context.Attributes, "data-val-notfutureyear-max", maxYear.ToString());
        }

        private static void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key))
            {
                attributes.Add(key, value);
            }
        }
    }
}
