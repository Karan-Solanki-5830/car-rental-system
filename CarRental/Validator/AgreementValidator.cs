using CarRental.Models;
using FluentValidation;

namespace CarRental.ValidationClass
{
    public class AgreementValidator : AbstractValidator<Agreement>
    {
        public AgreementValidator()
        {
            RuleFor(a => a.BookingId)
                .NotEmpty().WithMessage("Booking is required.");

            RuleFor(a => a.CustomerId)
                .NotEmpty().WithMessage("Customer is required.");

            RuleFor(a => a.AgreementDate)
                .NotEmpty().WithMessage("Agreement date is required.");

            RuleFor(a => a.AgreementPdfpath)
                .NotEmpty().WithMessage("Agreement PDF path is required.")
                .MaximumLength(255).WithMessage("PDF path must not exceed 255 characters.");

            RuleFor(a => a.TermsAccepted)
                .Equal(true).WithMessage("You must accept the terms.");
        }
    }
}
