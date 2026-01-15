using CarRental.Models;
using FluentValidation;

namespace CarRental.API.Validation
{
    public class FuelTypeValidator : AbstractValidator<FuelType>
    {
        public FuelTypeValidator()
        {
            RuleFor(f => f.FuelName)
                .NotEmpty().WithMessage("Fuel name is required.")
                .MaximumLength(50).WithMessage("Fuel name cannot exceed 50 characters.")
                .Matches("^[A-Za-z ]+$").WithMessage("Fuel name must contain only letters and spaces.");

            RuleFor(f => f.UserId)
                .GreaterThan(0).WithMessage("A valid UserId is required.");
        }
    }
}