
using FluentValidation;

namespace CodeBattleArena.Application.Features.Auth.Commands.Login
{
    public class LoginValidor : AbstractValidator<LoginCommand>
    {
        public LoginValidor() 
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email required")
                .EmailAddress().WithMessage("Incorrect email format")
                .MaximumLength(100).WithMessage("Email is too long");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cannot be empty");
        }
    }
}
