using FluentValidation;

namespace Application.Features.Authentication.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("Full name is required")
            .Length(2, 100).WithMessage("Full name must be between 2 and 100 characters")
            .Matches(@"^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ\s.-]+$").WithMessage("Full name can only contain letters, spaces, dots, and dashes");

        RuleFor(x => x.CorreoElectronico)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one digit");
    }
}