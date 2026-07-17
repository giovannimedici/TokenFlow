using FluentValidation;
using TokenFlow.API.DTOs;

namespace TokenFlow.API.Validators;

public class AuthValidator : AbstractValidator<AuthRequest>
{
    public AuthValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}