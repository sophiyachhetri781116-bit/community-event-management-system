using System.ComponentModel.DataAnnotations;

namespace CommunityEventManagement.Data.Models
{
    // Custom validation attribute: ensures an event's date is today or in the future.
    public class FutureOrTodayDateAttribute : ValidationAttribute
    {
        public FutureOrTodayDateAttribute()
        {
            ErrorMessage = "The event date cannot be in the past.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly date && date < DateOnly.FromDateTime(DateTime.Today))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
