
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.PlayerSessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionBestResult
{
    public class GetSessionBestResultHandler : IRequestHandler<GetSessionBestResultQuery, Result<PlayerSessionDto>>
    {
        private readonly IRepository<PlayerSession> _playerSessionRepository;
        private readonly IMapper _mapper;
        public GetSessionBestResultHandler(IRepository<PlayerSession> playerSessionRepository, IMapper mapper)
        {
            _playerSessionRepository = playerSessionRepository;
            _mapper = mapper;
        }
        public async Task<Result<PlayerSessionDto>> Handle(GetSessionBestResultQuery request, CancellationToken ct)
        {
            var bestPlayerSession = await _playerSessionRepository.GetBySpecAsync(new BestResultSpec(request.SessionId), ct);
            if (bestPlayerSession is null)
                return Result<PlayerSessionDto>.Failure(new Error("best_result.not_found", "Best result not found", 404));

                return Result<PlayerSessionDto>.Success(_mapper.Map<PlayerSessionDto>(bestPlayerSession));
        }
    }
}
