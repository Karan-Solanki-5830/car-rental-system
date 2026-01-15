using CarRental.Models;
using FluentValidation;

namespace CarRental.ValidationClass
{
    public class MaintenanceLogValidator : AbstractValidator<MaintenanceLog>
    {
        public MaintenanceLogValidator()
        {
            RuleFor(m => m.VehicleId)
                .GreaterThan(0).WithMessage("A valid VehicleId is required.");

            RuleFor(m => m.UserId)
                .GreaterThan(0).WithMessage("A valid UserId is required.");

            RuleFor(m => m.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(m => m.ServiceDate)
                .NotEmpty().WithMessage("Service date is required.");

            RuleFor(m => m.Cost)
                .GreaterThanOrEqualTo(0).WithMessage("Cost must be a non-negative value.");
        }
    }
}
