
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.StartGame
{
    public class StartGameHandler : IRequestHandler<StartGameCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionAccessService _accessService;
        public StartGameHandler(IUnitOfWork unitOfWork, ISessionAccessService sessionAccessService)
        {
            _unitOfWork = unitOfWork;
            _accessService = sessionAccessService;
        }
        public async Task<Result<bool>> Handle(StartGameCommand request, CancellationToken cancellationToken)
        {
            var sessionResult = await _accessService.GetSessionForUpdateAsync(request.Id, cancellationToken);

            if (sessionResult.IsFailure)
                return Result<bool>.Failure(sessionResult.Error);

            var session = sessionResult.Value;

            var result = session.Start();
            if (result.IsFailure) return Result<bool>.Failure(result.Error);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
