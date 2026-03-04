
using FluentValidation;

namespace CodeBattleArena.Application.Features.Auth.Commands.GoogleLogin
{
    public class GoogleLoginValidator : AbstractValidator<GoogleLoginCommand>
    {
        public GoogleLoginValidator() 
        { 
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Authorization code is required.");
        }
    }
}
