
using CodeBattleArena.Domain.Enums;
using FluentValidation;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.RequestAIProgrammingTask
{
    public class RequestAIProgrammingTaskValidator : AbstractValidator<RequestAIProgrammingTaskCommand>
    {
        public RequestAIProgrammingTaskValidator()
        {
            RuleFor(x => x.Difficulty)
                .NotEmpty().WithMessage("Difficulty is required.")
                .IsEnumName(typeof(Difficulty), caseSensitive: false)
                .WithMessage("Invalid difficulty level. Allowed values: Easy, Medium, Hard.");

            RuleFor(x => x.Prompt)
                .NotEmpty().WithMessage("Promt is required.")
                .MinimumLength(10).WithMessage("Promt is too short for AI generation.");
        }
    }
}
