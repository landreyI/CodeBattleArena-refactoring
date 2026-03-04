using FluentValidation;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.UpdateProgrammingTask
{
    public class UpdateProgrammingTaskValidator : AbstractValidator<UpdateProgrammingTaskCommand>
    {
        public UpdateProgrammingTaskValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .When(x => x.Name != null);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty if provided.")
                .When(x => x.Description != null);
        }
    }
}
