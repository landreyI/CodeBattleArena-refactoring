
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Players.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;
using MediatR;

namespace CodeBattleArena.Application.Features.Players.Queries.GetPlayer
{
    public class GetPlayerHandler : IRequestHandler<GetPlayerQuery, Result<PlayerDto>>
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IMapper _mapper;
        public GetPlayerHandler(IRepository<Player> playerRepository, IMapper mapper)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
        }

        public async Task<Result<PlayerDto>> Handle(GetPlayerQuery request, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetBySpecAsync(new PlayerReadOnlySpec(request.Id), cancellationToken);
            if (player is null)
                return Result<PlayerDto>.Failure(new Error("player.not_found", "Player not found", 404));

            return Result<PlayerDto>.Success(_mapper.Map<PlayerDto>(player));
        }
    }
}
