
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Leagues;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Commands.CreateLeague
{
    public class CreateLeagueHandler : IRequestHandler<CreateLeagueCommand, Result<Guid>>
    {
        private readonly IRepository<League> _leagueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateLeagueHandler(IRepository<League> leagueRepository, IUnitOfWork unitOfWork)
        {
            _leagueRepository = leagueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateLeagueCommand request, CancellationToken cancellationToken)
        {
            var resultLeague = League.Create(
                request.Name,
                request.MinWins,
                request.MaxWins,
                request.ImageUrl);

            if (resultLeague.IsFailure)
                return Result<Guid>.Failure(resultLeague.Error);

            await _leagueRepository.AddAsync(resultLeague.Value, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<Guid>.Success(resultLeague.Value.Id);
        }
    }
}
