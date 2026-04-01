
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Leagues.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Leagues;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Commands.DeleteLeague
{
    public class DeleteLeagueHandler : IRequestHandler<DeleteLeagueCommand, Result<bool>>
    {
        private readonly IRepository<League> _leagueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLeagueHandler(IRepository<League> leagueRepository, IUnitOfWork unitOfWork)
        {
            _leagueRepository = leagueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteLeagueCommand request, CancellationToken cancellationToken)
        {
            var league = await _leagueRepository.GetBySpecAsync(new LeagueForUpdateSpec(request.Id), cancellationToken);
            if (league == null)
                return Result<bool>.Failure(new Error("league.not_found", "Selected league not found", 404));

            _leagueRepository.Remove(league);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
