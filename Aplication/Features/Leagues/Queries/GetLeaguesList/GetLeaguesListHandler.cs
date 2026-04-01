
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Leagues.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Leagues;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Queries.GetLeaguesList
{
    public class GetLeaguesListHandler : IRequestHandler<GetLeaguesListQuery, Result<IReadOnlyList<LeagueDto>>>
    {
        private readonly IRepository<League> _leagueRepository;
        private readonly IMapper _mapper;

        public GetLeaguesListHandler(IRepository<League> leagueRepository, IMapper mapper)
        {
            _leagueRepository = leagueRepository;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<LeagueDto>>> Handle(GetLeaguesListQuery request, CancellationToken cancellationToken)
        {
            var leagues = await _leagueRepository.GetListBySpecAsync(new LeaguesListSpec(), cancellationToken);
            var leagueDtos = _mapper.Map<IReadOnlyList<LeagueDto>>(leagues);
            return Result<IReadOnlyList<LeagueDto>>.Success(leagueDtos);
        }
    }
}
