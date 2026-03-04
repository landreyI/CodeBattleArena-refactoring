using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.DeleteSession
{
    public class DeleteSessionHandler : IRequestHandler<DeleteSessionCommand, Result<bool>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly ISessionAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteSessionHandler(
            IRepository<Session> sessionRepository,
            ISessionAccessService sessionAccessService,
            IUnitOfWork unitOfWork)
        {
            _sessionRepository = sessionRepository;
            _accessService = sessionAccessService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
        {
            var sessionResult = await _accessService.GetSessionForUpdateAsync(request.Id, cancellationToken);

            if (sessionResult.IsFailure)
                return Result<bool>.Failure(sessionResult.Error);

            var session = sessionResult.Value;

            session.Delete();
            _sessionRepository.Remove(session);
            
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
