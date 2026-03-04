
using FluentValidation;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTask
{
    public class GetPlayerProgrammingTasksListValidator : AbstractValidator<GetProgrammingTaskQuery>
    {
        public GetPlayerProgrammingTasksListValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ProgrammingTaskId is required.");
        }
    }
}
