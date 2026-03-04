
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.RequestAIProgrammingTask
{
    public class RequestAIProgrammingTaskCommand : IRequest<Result<Guid>>
    {
        public Guid? ExistingId { get; set; }
        public List<Guid>? ProgrammingLangIds { get; init; } // если null - ИИ создаст задачу на всех поддерживаемых языках
        public string Difficulty { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
    }
}
