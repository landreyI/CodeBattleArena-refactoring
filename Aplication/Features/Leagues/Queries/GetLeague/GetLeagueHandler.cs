
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Leagues.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Leagues;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Queries.GetLeague
{
    public class GetLeagueHandler : IRequestHandler<GetLeagueQuery, Result<LeagueDto>>
    {
        private readonly IRepository<League> _leagueRepository;
        private readonly IMapper _mapper;

        public GetLeagueHandler(IRepository<League> leagueRepository, IMapper mapper)
        {
            _leagueRepository = leagueRepository;
            _mapper = mapper;
        }

        public async Task<Result<LeagueDto>> Handle(GetLeagueQuery request, CancellationToken cancellationToken)
        {
            var league = await _leagueRepository.GetBySpecAsync(new LeagueReadOnlySpec(request.Id), cancellationToken);
            if (league is null) 
                return Result<LeagueDto>.Failure(new Error("league.not_found", "League not found", 404));

            return Result<LeagueDto>.Success(_mapper.Map<LeagueDto>(league));
        }
    }
}
