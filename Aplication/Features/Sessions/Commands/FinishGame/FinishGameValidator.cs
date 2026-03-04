using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Commands.FinishGame
{
    public class StartGameValidator : AbstractValidator<FinishGameCommand>
    {
        public StartGameValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("SessionId is required.");
        }
    }
}
