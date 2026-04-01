
using FluentValidation;

namespace CodeBattleArena.Application.Features.Leagues.Commands.CreateLeague
{
    public class CreateLeagueValidator : AbstractValidator<CreateLeagueCommand>
    {
        public CreateLeagueValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("League name is required.")
                .MaximumLength(30).WithMessage("League name cannot exceed 30 characters.");

            // Validating the S3/External URL if it exists
            RuleFor(x => x.ImageUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .WithMessage("ImageUrl must be a valid absolute URL.")
                .MaximumLength(500).WithMessage("League URL is too long (max 500 characters).");

            RuleFor(x => x.MinWins)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum wins cannot be negative.");

            RuleFor(x => x.MaxWins)
                .GreaterThanOrEqualTo(x => x.MinWins).WithMessage("Maximum wins must be greater than the minimum wins.");
        }
    }
}
