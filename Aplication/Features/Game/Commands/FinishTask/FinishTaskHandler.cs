
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Game.Commands.FinishTask
{
    public class FinishTaskHandler : IRequestHandler<FinishTaskCommand, Result<bool>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public FinishTaskHandler(IRepository<Session> sessionRepository, IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(FinishTaskCommand request, CancellationToken cancellationToken)
        {
            var currentPlayerId = _identityService.CurrentPlayerId();
            if (!currentPlayerId.HasValue)
                return Result<bool>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var spec = new ActiveSessionForUpdateSpec(currentPlayerId.Value);
            var activeSession = await _sessionRepository.GetBySpecAsync(spec, cancellationToken);
            if (activeSession is null)
                return Result<bool>.Failure(new Error("session.active", "Active session not found", 404));

            var resultUpdate = activeSession.FinishTaskPlayer(currentPlayerId.Value);
            if (resultUpdate.IsFailure)
                return Result<bool>.Failure(resultUpdate.Error);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
