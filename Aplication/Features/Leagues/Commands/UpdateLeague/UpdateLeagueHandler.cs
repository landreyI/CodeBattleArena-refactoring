
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Features.Leagues.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Leagues;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Commands.UpdateLeague
{
    public class UpdateLeagueHandler : IRequestHandler<UpdateLeagueCommand, Result<bool>>
    {
        private readonly IRepository<League> _leagueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLeagueHandler(IRepository<League> leagueRepository, IUnitOfWork unitOfWork)
        {
            _leagueRepository = leagueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateLeagueCommand request, CancellationToken cancellationToken)
        {
            var league = await _leagueRepository.GetBySpecAsync(new LeagueForUpdateSpec(request.Id), cancellationToken);
            if (league == null)
                return Result<bool>.Failure(new Error("league.not_found", "Selected league not found", 404));

            var resultUpdate = league.Update(
                request.Name,
                request.MinWins,
                request.MaxWins,
                request.ImageUrl);

            if (resultUpdate.IsFailure)
                return Result<bool>.Failure(resultUpdate.Error);

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
