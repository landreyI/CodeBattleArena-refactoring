
using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Players.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using MediatR;

namespace CodeBattleArena.Application.Features.Players.Queries.GetPlayersList
{
    public class GetPlayersListHandler : IRequestHandler<GetPlayersListQuery, Result<PaginatedResult<PlayerDto>>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IMapper _mapper;
        public GetPlayersListHandler(IRepository<Player> playerRepository, IMapper mapper)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<PlayerDto>>> Handle(GetPlayersListQuery request, CancellationToken ct)
        {
            var spec = new PlayersListSpec(request.Filter);

            var players = await _playerRepository.GetListBySpecAsync(spec, ct);

            var totalCount = await _playerRepository.CountAsync(spec, ct);

            var dtos = _mapper.Map<List<PlayerDto>>(players);

            var result = new PaginatedResult<PlayerDto>(
                dtos,
                totalCount,
                request.Filter?.PageNumber ?? 1,
                request.Filter?.PageSize ?? 30);

            return Result<PaginatedResult<PlayerDto>>.Success(result);
        }
    }
}
