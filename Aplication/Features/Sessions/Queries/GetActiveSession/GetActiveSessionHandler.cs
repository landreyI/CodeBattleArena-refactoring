using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetActiveSession
{
    public class GetActiveSessionHandler : IRequestHandler<GetActiveSessionQuery, Result<SessionDto?>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;

        public GetActiveSessionHandler(IRepository<Session> sessionRepository, IMapper mapper, IIdentityService identityService)
        {
            _sessionRepository = sessionRepository;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<SessionDto?>> Handle(GetActiveSessionQuery request, CancellationToken cancellationToken)
        {
            var currentPlayerId = _identityService.CurrentPlayerId();
            if (!currentPlayerId.HasValue)
                return Result<SessionDto?>.Failure(new Error("auth.unauthorized", "User not found in context", 401));

            var spec = new ActiveSessionReadOnlySpec(currentPlayerId.Value);
            var activeSession = await _sessionRepository.GetBySpecAsync(spec, cancellationToken);
            if (activeSession is null)
                return Result<SessionDto?>.Failure(new Error("session.active", "Active session not found", 200));

            return Result<SessionDto?>.Success(_mapper.Map<SessionDto>(activeSession));
        }
    }
}
