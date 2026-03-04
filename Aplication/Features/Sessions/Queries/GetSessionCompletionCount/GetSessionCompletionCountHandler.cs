using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Models;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionCompletionCount
{
    public class GetSessionCompletionCountHandler : IRequestHandler<GetSessionCompletionCountQuery, Result<CompletionCount>>
    {
        private readonly IRepository<Session> _sessionRepository;

        public GetSessionCompletionCountHandler(IRepository<Session> sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<Result<CompletionCount>> Handle(GetSessionCompletionCountQuery request, CancellationToken cancellationToken)
        {
            var sessionSpec = new SessionReadOnlySpec(request.SessionId);
            var session = await _sessionRepository.GetBySpecAsync(sessionSpec, cancellationToken);
            if (session is null)
                return Result<CompletionCount>.Failure(new Error("session.not_found", "Session not found", 404));

            var result = session.GetCompletionCount();
            if(result.IsFailure)
                return Result<CompletionCount>.Failure(result.Error);

            return Result<CompletionCount>.Success(result.Value);
        }
    }
}
