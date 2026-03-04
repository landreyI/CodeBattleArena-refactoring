using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.PlayerSessions;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.JoinSession
{
    public class JoinSessionHandler : IRequestHandler<JoinSessionCommand, Result<bool>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public JoinSessionHandler(IRepository<Session> sessionRepository, IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(JoinSessionCommand request, CancellationToken cancellationToken)
        {
            var currentPlayerId = _identityService.CurrentPlayerId();
            if (!currentPlayerId.HasValue)
                return Result<bool>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var specHasActive = new ActiveSessionReadOnlySpec(currentPlayerId.Value);
            var hasActive = await _sessionRepository.AnyAsync(specHasActive, cancellationToken);
            if (hasActive)
                return Result<bool>.Failure(new Error("session.active", "Player already has an active session", 400));

            var spec = new SessionForUpdateSpec(request.Id);

            var session = await _sessionRepository.GetBySpecAsync(spec, cancellationToken);
            if (session is null)
                return Result<bool>.Failure(new Error("session.not_found", "Session not found", 404));

            var resultPlayerSession = PlayerSession.Create(currentPlayerId.Value, session.Id);
            if (resultPlayerSession.IsFailure)
                return Result<bool>.Failure(resultPlayerSession.Error);

            var resultAdd = session.AddPlayer(resultPlayerSession.Value, request.Password);
            if (resultAdd.IsFailure)
                return Result<bool>.Failure(resultAdd.Error);

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
