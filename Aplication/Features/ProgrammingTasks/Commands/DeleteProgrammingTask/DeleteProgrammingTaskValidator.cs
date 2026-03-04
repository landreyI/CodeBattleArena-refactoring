using FluentValidation;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.DeleteProgrammingTask
{
    public class DeleteProgrammingTaskValidator : AbstractValidator<DeleteProgrammingTaskCommand>
    {
        public DeleteProgrammingTaskValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ProgrammingTaskId is required.");
        }
    }
}
