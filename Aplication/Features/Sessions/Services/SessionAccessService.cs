using CodeBattleArena.Application.Common.Helpers;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;


namespace CodeBattleArena.Application.Features.Sessions.Services
{
    public class SessionAccessService : ISessionAccessService
    {
        private readonly IIdentityService _identityService;
        private readonly IRepository<Session> _sessionRepository;

        public SessionAccessService(IIdentityService identityService, IRepository<Session> sessionRepository)
        {
            _identityService = identityService;
            _sessionRepository = sessionRepository;
        }

        public async Task<Result<Session>> GetSessionForUpdateAsync(Guid sessionId, CancellationToken ct)
        {
            var user = await _identityService.GetUserContextAsync();
            if (!user.IsAuthenticated)
                return Result<Session>.Failure(new Error("auth.unauthorized", "User not found", 401));

            var spec = new SessionForUpdateSpec(sessionId);
            var session = await _sessionRepository.GetBySpecAsync(spec, ct);

            if (session is null)
                return Result<Session>.Failure(new Error("session.not_found", "Session not found", 404));

            if (!SessionAccessPolicy.CanEditSession(session, user))
                return Result<Session>.Failure(new Error("session.forbidden", "No permission to edit", 403));

            return Result<Session>.Success(session);
        }
    }
}
