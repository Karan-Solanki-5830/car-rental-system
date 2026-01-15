using CarRental.Models;
using FluentValidation;

namespace CarRental.ValidationClass
{
    public class PaymentValidator : AbstractValidator<Payment>
    {
        public PaymentValidator()
        {
            RuleFor(p => p.BookingId)
                .GreaterThan(0).WithMessage("A valid BookingId is required.");

            RuleFor(p => p.UserId)
                .GreaterThan(0).WithMessage("A valid UserId is required.");

            RuleFor(p => p.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(p => p.PaymentDate)
                .NotEmpty().WithMessage("Payment date is required.");

            RuleFor(p => p.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .MaximumLength(50).WithMessage("Payment method must not exceed 50 characters.");

            RuleFor(p => p.Remarks)
                .MaximumLength(255).WithMessage("Remarks must not exceed 255 characters.");
        }
    }
}
