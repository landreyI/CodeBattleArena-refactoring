using Ardalis.Specification;
using CodeBattleArena.Domain.Sessions;

namespace CodeBattleArena.Application.Features.Sessions.Specifications
{
    public abstract class SessionBaseSpec : Specification<Session>
    {
        protected SessionBaseSpec() {} // для других критериев

        protected SessionBaseSpec(Guid sessionId) : this()
        {
            Query.Where(s => s.Id == sessionId);
        }

        protected void AddCommonIncludes()
        {
            Query.Include(s => s.ProgrammingLang)
                 .Include(s => s.ProgrammingTask)
                 .Include(s => s.PlayerSessions);
        }
    }
}