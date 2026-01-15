using CarRental.Models;
using FluentValidation;

namespace CarRental.ValidationClass
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
                .Matches("^[a-zA-Z ]+$").WithMessage("Name must contain only letters and spaces.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");

            RuleFor(u => u.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^[0-9]{10}$").WithMessage("Phone must be a 10-digit number.");

            RuleFor(u => u.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(role => new[] { "Admin", "Customer" }.Contains(role))
                .WithMessage("Role must be one of the following: Admin, Customer.");
        }
    }
}
