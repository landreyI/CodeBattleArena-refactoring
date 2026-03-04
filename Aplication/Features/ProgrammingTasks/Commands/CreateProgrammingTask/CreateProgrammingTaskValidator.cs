
using FluentValidation;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.CreateProgrammingTask
{
    public class CreateProgrammingTaskValidator : AbstractValidator<CreateProgrammingTaskCommand>
    {
        public CreateProgrammingTaskValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().MaximumLength(100)
                .WithMessage("Name task cannot be longer than 100 characters..");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.");
        }
    }
}
