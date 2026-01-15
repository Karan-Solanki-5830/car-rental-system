using CarRental.Models;
using FluentValidation;

namespace CarRental.Validation
{
    public class VehicleValidator : AbstractValidator<Vehicle>
    {
        public VehicleValidator()
        {
            RuleFor(v => v.Brand)
                .NotEmpty().WithMessage("Brand is required.")
                .MaximumLength(50).WithMessage("Brand cannot exceed 50 characters.");

            RuleFor(v => v.Model)
                .NotEmpty().WithMessage("Model is required.")
                .MaximumLength(50).WithMessage("Model cannot exceed 50 characters.");

            RuleFor(v => v.Year)
                .InclusiveBetween(1990, DateTime.Now.Year + 1)
                .WithMessage($"Year must be between 1990 and {DateTime.Now.Year + 1}.");

            RuleFor(v => v.PlateNumber)
                .NotEmpty().WithMessage("Plate number is required.")
                .MaximumLength(20).WithMessage("Plate number cannot exceed 20 characters.");

            RuleFor(v => v.FuelTypeId)
                .GreaterThan(0).WithMessage("Fuel type must be selected.");

            RuleFor(v => v.VehicleTypeId)
                .GreaterThan(0).WithMessage("Vehicle type must be selected.");

            RuleFor(v => v.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => s.ToLower() == "available" || s.ToLower() == "unavailable")
                .WithMessage("Status must be either 'available' or 'unavailable'.");

            RuleFor(v => v.Mileage)
                .GreaterThanOrEqualTo(0).WithMessage("Mileage must be a non-negative number.");

            RuleFor(v => v.ConditionNote)
                .NotEmpty().WithMessage("Condition note is required.")
                .MaximumLength(500).WithMessage("Condition note cannot exceed 500 characters.");

            RuleFor(v => v.PricePerHour)
                .GreaterThan(0).WithMessage("Price per hour must be greater than 0.");

            RuleFor(v => v.PricePerDay)
                .GreaterThan(0).WithMessage("Price per day must be greater than 0.");
        }
    }
}
