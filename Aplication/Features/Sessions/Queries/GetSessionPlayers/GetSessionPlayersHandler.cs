using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.PlayerSessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionPlayers
{
    public class GetSessionPlayersHandler : IRequestHandler<GetSessionPlayersQuery, Result<List<PlayerSessionDto>>>
    {
        private readonly IRepository<PlayerSession> _playerSessionRepository;
        private readonly IMapper _mapper;
        public GetSessionPlayersHandler(IRepository<PlayerSession> playerSessionRepository, IMapper mapper)
        {
            _playerSessionRepository = playerSessionRepository;
            _mapper = mapper;
        }
        public async Task<Result<List<PlayerSessionDto>>> Handle(GetSessionPlayersQuery request, CancellationToken cancellationToken)
        {
            var spec = new PlayerSessionIncludesSpec(request.Id);
            var playerSessions = await _playerSessionRepository.GetBySpecAsync(spec, cancellationToken);
            return Result<List<PlayerSessionDto>>.Success(_mapper.Map<List<PlayerSessionDto>>(playerSessions));
        }
    }
}
