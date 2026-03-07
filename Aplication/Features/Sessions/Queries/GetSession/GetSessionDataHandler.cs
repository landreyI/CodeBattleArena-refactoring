using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSession
{
    public class GetSessionDataHandler : IRequestHandler<GetSessionDataQuery, Result<Session>>
    {
        private readonly IRepository<Session> _sessionRepository;
        public GetSessionDataHandler(IRepository<Session> sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<Result<Session>> Handle(GetSessionDataQuery request, CancellationToken ct)
        {
            var spec = new SessionReadOnlySpec(request.Id);

            var session = await _sessionRepository.GetBySpecAsync(spec, ct);
            if (session is null)
                return Result<Session>.Failure(new Error("session.not_found", "Session not found", 404));

            return Result<Session>.Success(session);
        }
    }
}
