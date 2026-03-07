using AutoMapper;
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetPlayerSessionHistory
{
    public class GetPlayerSessionHistoryHandler
    : IRequestHandler<GetPlayerSessionHistoryQuery, Result<PaginatedResult<SessionDto>>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IMapper _mapper;
        public GetPlayerSessionHistoryHandler(IRepository<Session> sessionRepository, IMapper mapper)
        {
            _sessionRepository = sessionRepository;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedResult<SessionDto>>> Handle(GetPlayerSessionHistoryQuery request, CancellationToken ct)
        {
            var spec = new SessionsListSpec(request.PlayerId, request.Filter);
            var sessions = await _sessionRepository.GetListBySpecAsync(spec, ct);
            var totalCount = await _sessionRepository.CountAsync(spec, ct);

            var dtos = _mapper.Map<List<SessionDto>>(sessions);

            var result = new PaginatedResult<SessionDto>(
                dtos,
                totalCount,
                request.Filter?.PageNumber ?? 1,
                request.Filter?.PageSize ?? 15);

            return Result<PaginatedResult<SessionDto>>.Success(result);
        }
    }
}
