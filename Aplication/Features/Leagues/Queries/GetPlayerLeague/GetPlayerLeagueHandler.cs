
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Leagues;
using CodeBattleArena.Domain.Players;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Queries.GetPlayerLeague
{
    public class GetPlayerLeagueHandler : IRequestHandler<GetPlayerLeagueQuery, Result<LeagueDto>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<League> _leagueRepository;
        private readonly IMapper _mapper;

        public GetPlayerLeagueHandler(IRepository<Player> playerRepository, IRepository<League> leagueRepository, IMapper mapper)
        {
            _playerRepository = playerRepository;
            _leagueRepository = leagueRepository;
            _mapper = mapper;
        }

        public async Task<Result<LeagueDto>> Handle(GetPlayerLeagueQuery request, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetByIdAsync(request.PlayerId, true, cancellationToken);
            if (player is null)
                return Result<LeagueDto>.Failure(new Error("player.not_found", "Player not found", 404));

            var league = await _leagueRepository.GetByIdAsync(player.LeagueId.Value, true, cancellationToken);
            if (league is null)
                return Result<LeagueDto>.Failure(new Error("league.not_found", "League not found", 404));

            return Result<LeagueDto>.Success(_mapper.Map<LeagueDto>(league));
        }
    }
}
