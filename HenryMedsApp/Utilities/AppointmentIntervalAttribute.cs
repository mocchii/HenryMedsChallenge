using System.ComponentModel.DataAnnotations;

namespace HenryMedsApp.Utilities
{
    public class AppointmentIntervalAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime.Minute % 15 == 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("The time must be in 15-minute intervals.");
                }
            }
            return new ValidationResult("Invalid DateTime format.");
        }
    }
}
