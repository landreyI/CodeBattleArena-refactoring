
using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Commands.InviteSession
{
    public class InviteSessionValidator : AbstractValidator<InviteSessionCommand>
    {
        public InviteSessionValidator() 
        {
            RuleFor(x => x.SessionId).NotEmpty().WithMessage("SessionId is required.");

            RuleFor(x => x.PlayerIds)
                .NotNull().WithMessage("Player list cannot be null.")
                .NotEmpty().WithMessage("Please select at least one player to invite.")
                .Must(list => list.Count <= 10).WithMessage("You cannot invite more than 10 players at once.");

            RuleForEach(x => x.PlayerIds)
                .NotEmpty().WithMessage("Player ID cannot be empty.")
                .Must(id => id != Guid.Empty).WithMessage("Invalid Player ID format.");
        }
    }
}
