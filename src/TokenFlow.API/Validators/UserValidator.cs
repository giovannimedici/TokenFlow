using FluentValidation;
using TokenFlow.API.DTOs;

namespace TokenFlow.API.Validators;

public class CreateUserValidator : AbstractValidator<UserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
            .MaximumLength(50).WithMessage("Username must be at most 50 characters long")
            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Username must contain only letters and numbers");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .MaximumLength(50).WithMessage("Password must be at most 50 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}