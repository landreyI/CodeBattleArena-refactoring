
using FluentValidation;

namespace CodeBattleArena.Application.Features.Items.Commands.UpdateItem
{
    public class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ItemId is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Item name is required.")
                .MaximumLength(100).WithMessage("Item name cannot exceed 100 characters.")
                .When(x => x.Name != null);

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid item type specified.")
                .When(x => x.Name != null);

            RuleFor(x => x.PriceCoin)
                .GreaterThanOrEqualTo(0).When(x => x.PriceCoin.HasValue)
                .WithMessage("Price cannot be negative.");

            // Validating the S3/External URL if it exists
            RuleFor(x => x.ImageUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .WithMessage("ImageUrl must be a valid absolute URL.")
                .MaximumLength(500).WithMessage("Image URL is too long (max 500 characters).");

            RuleFor(x => x.CssClass)
                .MaximumLength(50).WithMessage("CSS class name cannot exceed 50 characters.")
                .When(x => x.Name != null);

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(x => x.Name != null);
        }
    }
}
