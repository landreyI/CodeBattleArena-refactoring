
using Ardalis.Specification;
using CodeBattleArena.Domain.Sessions;

namespace CodeBattleArena.Application.Features.Sessions.Specifications
{
    public class SessionForUpdateSpec : SessionBaseSpec, ISingleResultSpecification<Session>
    {
        public SessionForUpdateSpec(Guid sessionId) : base(sessionId)
        {
            // IsTracking = true по умолчанию
            AddCommonIncludes();
        }
    }
}
