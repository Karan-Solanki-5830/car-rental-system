using CarRental.Models;
using FluentValidation;

namespace CarRental.ValidationClass
{
    public class BookingValidator : AbstractValidator<Booking>
    {
        public BookingValidator()
        {
            RuleFor(b => b.CustomerId)
                .GreaterThan(0).WithMessage("A valid CustomerId is required.");

            RuleFor(b => b.VehicleId)
                .GreaterThan(0).WithMessage("A valid VehicleId is required.");

            RuleFor(b => b.StartDateTime)
                .LessThan(b => b.EndDateTime).WithMessage("Start date/time must be before end date/time.");

            RuleFor(b => b.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MaximumLength(50).WithMessage("Status must not exceed 50 characters.");
        }
    }
}
