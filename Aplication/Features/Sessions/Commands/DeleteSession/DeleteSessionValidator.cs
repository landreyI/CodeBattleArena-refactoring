using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Commands.DeleteSession
{
    public class DeleteSessionValidator : AbstractValidator<DeleteSessionCommand>
    {
        public DeleteSessionValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("SessionId is required.");
        }
    }
}
