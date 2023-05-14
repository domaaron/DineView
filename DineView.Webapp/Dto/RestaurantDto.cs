using DineView.Application.models;
using System;
using System.ComponentModel.DataAnnotations;

namespace DineView.Webapp.Dto
{
    class ValidOpeningTime : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var restaurant = validationContext.ObjectInstance as RestaurantDto;
            if (restaurant is null) { return null; }

            if (restaurant.OpeningTime < TimeOnly.MinValue.AddHours(8))
            {
                return new ValidationResult("Opening time can be no earlier than 8 am");
            }

            return ValidationResult.Success;
        }
    }

    class ValidClosedTime: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var restaurant = validationContext.ObjectInstance as RestaurantDto;
            if (restaurant is null) { return null; }

            if (restaurant.ClosedTime > TimeOnly.MaxValue)
            {
                return new ValidationResult("Closing time can be no later than 12 pm");
            }

            return ValidationResult.Success;
        }
    }

    public record RestaurantDto
    (
        Guid Guid,
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        string Name,
        [ValidOpeningTime]
        TimeOnly OpeningTime,
        [ValidClosedTime]
        TimeOnly ClosedTime,
        bool IsOrderable,
        [DataType(DataType.PhoneNumber)]
        string Tel,
        [Url(ErrorMessage = "URL is not valid")]
        string URL
    );
}
