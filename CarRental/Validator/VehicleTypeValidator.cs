using CarRental.Models;
using FluentValidation;

namespace CarRental.ValidationClass
{
    public class VehicleTypeValidator : AbstractValidator<VehicleType>
    {
        public VehicleTypeValidator()
        {
            RuleFor(v => v.TypeName)
                .NotEmpty().WithMessage("Type name is required.")
                .MaximumLength(50).WithMessage("Type name must not exceed 50 characters.");

            RuleFor(v => v.UserId)
                .GreaterThan(0).WithMessage("A valid UserId is required.");
        }
    }
}
