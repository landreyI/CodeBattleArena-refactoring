
using FluentValidation;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTask
{
    public class GetProgrammingTaskValidator : AbstractValidator<GetProgrammingTaskQuery>
    {
        public GetProgrammingTaskValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ProgrammingTaskId is required.");
        }
    }
}
