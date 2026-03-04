using AutoMapper;
using CodeBattleArena.Application.Common.Helpers;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Models;
using CodeBattleArena.Application.Features.Sessions.Specifications;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSession
{
    public class GetSessionHandler : IRequestHandler<GetSessionQuery, Result<SessionDtoAndIsEdit>>
    {
        private readonly IRepository<Session> _sessionRepository;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;

        public GetSessionHandler(IRepository<Session> sessionRepository, IMapper mapper, IIdentityService identityService)
        {
            _sessionRepository = sessionRepository;
            _mapper = mapper;
            _identityService = identityService;
        }
        public async Task<Result<SessionDtoAndIsEdit>> Handle(GetSessionQuery request, CancellationToken ct)
        {
            var spec = new SessionReadOnlySpec(request.Id);

            var session = await _sessionRepository.GetBySpecAsync(spec, ct);
            if (session is null)
                return Result<SessionDtoAndIsEdit>.Failure(new Error("session.not_found", "Session not found", 404));

            var user = await _identityService.GetUserContextAsync();
            bool isEdit = SessionAccessPolicy.CanEditSession(session, user);

            return Result<SessionDtoAndIsEdit>.Success(new SessionDtoAndIsEdit(_mapper.Map<SessionDto>(session), isEdit));
        }
    }
}
