
using Ardalis.Specification;
using CodeBattleArena.Domain.Sessions;

namespace CodeBattleArena.Application.Features.Sessions.Specifications
{
    public class SessionReadOnlySpec : SessionBaseSpec, ISingleResultSpecification<Session>
    {
        public SessionReadOnlySpec(Guid sessionId) : base(sessionId)
        {
            AddCommonIncludes();
            Query.AsNoTracking();
        }
    }
}
