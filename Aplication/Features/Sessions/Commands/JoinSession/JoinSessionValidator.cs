using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Commands.JoinSession
{
    public class FinishGameValidator : AbstractValidator<JoinSessionCommand>
    {
        public FinishGameValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("SessionId is required.");
        }
    }
}
