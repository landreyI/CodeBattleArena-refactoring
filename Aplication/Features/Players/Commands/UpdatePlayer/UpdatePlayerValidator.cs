
using FluentValidation;

namespace CodeBattleArena.Application.Features.Players.Commands.UpdatePlayer
{
    public class UpdatePlayerValidator : AbstractValidator<UpdatePlayerCommand>
    {
        public UpdatePlayerValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("PlayerId is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Player name is required.")
                .MaximumLength(30).WithMessage("Player name cannot exceed 30 characters.")
                .When(x => x.Name != null);

            RuleFor(x => x.Info)
                .IsInEnum().WithMessage("Invalid item type specified.")
                .When(x => x.Name != null);

            // Validating the S3/External URL if it exists
            RuleFor(x => x.PhotoUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrWhiteSpace(x.PhotoUrl))
                .WithMessage("ImageUrl must be a valid absolute URL.")
                .MaximumLength(500).WithMessage("Image URL is too long (max 500 characters).");
        }
    }
}
