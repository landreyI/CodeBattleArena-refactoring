using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionsList
{
    public class GetProgrammingTasksListHandler : IRequestHandler<GetSessionsListQuery, Result<List<SessionDto>>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IMapper _mapper;
        public GetProgrammingTasksListHandler(IRepository<Session> sessionRepository, IMapper mapper) 
        { 
            _sessionRepository = sessionRepository;
            _mapper = mapper;
        }
        public async Task<Result<List<SessionDto>>> Handle(GetSessionsListQuery request, CancellationToken ct)
        {
            var spec = new Specifications.SessionsListSpec(request.Filter);
            var sessions = await _sessionRepository.GetListBySpecAsync(spec, ct);
            return Result<List<SessionDto>>.Success(_mapper.Map<List<SessionDto>>(sessions));
        }
    }
}
