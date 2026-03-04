using FluentValidation;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetPlayerProgrammingTasksList
{
    public class GetPlayerProgrammingTasksListValidator : AbstractValidator<GetPlayerProgrammingTasksListQuery>
    {
        public GetPlayerProgrammingTasksListValidator()
        {
            RuleFor(x => x.PlayerId).NotEmpty().WithMessage("PlayerId is required.");
        }
    }
}
