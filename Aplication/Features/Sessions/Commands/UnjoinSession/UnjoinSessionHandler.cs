
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

namespace CodeBattleArena.Application.Features.Sessions.Commands.UnjoinSession
{
    public class UnjoinSessionHandler : IRequestHandler<UnjoinSessionCommand, Result<bool>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;
        public UnjoinSessionHandler(IRepository<Session> sessionRepository, IIdentityService identityService, IUnitOfWork unitOfWork, HybridCache cache)
        {
            _sessionRepository = sessionRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }
        public async Task<Result<bool>> Handle(UnjoinSessionCommand request, CancellationToken cancellationToken)
        {
            var currentPlayerId = _identityService.CurrentPlayerId();
            if (!currentPlayerId.HasValue)
                return Result<bool>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var spec = new ActiveSessionForUpdateSpec(currentPlayerId.Value);
            var activeSession = await _sessionRepository.GetBySpecAsync(spec, cancellationToken);
            if (activeSession is null)
                return Result<bool>.Failure(new Error("session.active", "Active session not found", 404));

            var resultRemovePlayer = activeSession.RemovePlayer(currentPlayerId.Value);
            if (resultRemovePlayer.IsFailure)
                return Result<bool>.Failure(resultRemovePlayer.Error);

            if(activeSession.PlayerSessions?.Any() != true)
                _sessionRepository.Remove(activeSession);

            await _unitOfWork.CommitAsync(cancellationToken);

            await _cache.RemoveAsync(CacheKeys.Sessions.Details(activeSession.Id), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Sessions.ListTag, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
