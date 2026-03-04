using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.PlayerSessions;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.CreateSession
{
    public class CreateSessionHandler : IRequestHandler<CreateSessionCommand, Result<Guid>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        public CreateSessionHandler(IRepository<Session> sessionRepository, IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        {
            var currentPlayerId = _identityService.CurrentPlayerId();
            if(!currentPlayerId.HasValue)
                return Result<Guid>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var spec = new ActiveSessionReadOnlySpec(currentPlayerId.Value);
            var hasActive = await _sessionRepository.AnyAsync(spec, cancellationToken);
            if (hasActive)
                return Result<Guid>.Failure(new Error("session.active", "Player already has an active session", 400));


            var resultSession = Session.Create(
                request.Name, 
                currentPlayerId.Value,
                request.State, 
                request.MaxPeople, 
                request.Password, 
                request.ProgrammingLangId, 
                request.TimePlay);

            if(resultSession.IsFailure)
                return Result<Guid>.Failure(resultSession.Error);

            var resultPlayerSession = PlayerSession.Create(currentPlayerId.Value, resultSession.Value.Id);
            if(resultPlayerSession.IsFailure)
                return Result<Guid>.Failure(resultPlayerSession.Error);

            var resultAdd = resultSession.Value.AddPlayer(resultPlayerSession.Value);
            if(resultAdd.IsFailure)
                return Result<Guid>.Failure(resultAdd.Error);

            await _sessionRepository.AddAsync(resultSession.Value, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Success(resultSession.Value.Id);
        }
    }
}
