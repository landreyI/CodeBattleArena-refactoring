
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Sessions.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.FinishGame
{
    public class FinishGameHandler : IRequestHandler<FinishGameCommand, Result<bool>>
    {
        private readonly ISessionAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;
        public FinishGameHandler(IUnitOfWork unitOfWork, ISessionAccessService accessService)
        {
            _unitOfWork = unitOfWork;
            _accessService = accessService;
        }
        public async Task<Result<bool>> Handle(FinishGameCommand request, CancellationToken cancellationToken)
        {
            var sessionResult = await _accessService.GetSessionForUpdateAsync(request.Id, cancellationToken);

            if (sessionResult.IsFailure)
                return Result<bool>.Failure(sessionResult.Error);

            var session = sessionResult.Value;

            var result = session.Finish();
            if (result.IsFailure) return Result<bool>.Failure(result.Error);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
