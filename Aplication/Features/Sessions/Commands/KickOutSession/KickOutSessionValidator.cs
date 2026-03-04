
using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Commands.KickOutSession
{
    public class KickOutSessionValidator : AbstractValidator<KickOutSessionCommand>
    {
        public KickOutSessionValidator()
        {
            RuleFor(x => x.SessionId).NotEmpty().WithMessage("Session ID is required.");
            RuleFor(x => x.PlayerId).NotEmpty().WithMessage("Player ID is required.");
        }
    }
}
