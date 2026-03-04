using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Commands.StartGame
{
    public class StartGameValidator : AbstractValidator<StartGameCommand>
    {
        public StartGameValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("SessionId is required.");
        }
    }
}
