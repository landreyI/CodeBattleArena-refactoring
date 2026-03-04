using FluentValidation;

namespace CodeBattleArena.Application.Features.Sessions.Commands.UpdateSession
{
    public class UpdateSessionValidator : AbstractValidator<UpdateSessionCommand>
    {
        public UpdateSessionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().MaximumLength(20)
                .WithMessage("Name session cannot be longer than 20 characters..");

            RuleFor(x => x.TimePlay)
                .InclusiveBetween(5, 180)
                .WithMessage("The game duration should be between 5 and 180 minutes.")
                .When(x => x.TimePlay.HasValue);

            RuleFor(x => x.MaxPeople)
                .InclusiveBetween(1, 10)
                .WithMessage("The number of participants must be greater than zero and less than 11.");
        }
    }
}
