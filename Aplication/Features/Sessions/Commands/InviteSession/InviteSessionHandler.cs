
using AutoMapper;
using CodeBattleArena.Application.Common.Interfaces.Notifications;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.InviteSession
{
    public class InviteSessionHandler : IRequestHandler<InviteSessionCommand, Result<bool>>
    {
        private readonly ISessionAccessService _accessService;
        private readonly IPlayerNotificationService _playerNotificationService;
        private readonly IMapper _mapper;

        public InviteSessionHandler(
            ISessionAccessService accessService,
            IPlayerNotificationService playerNotificationService,
            IMapper mapper)
        {
            _accessService = accessService;
            _playerNotificationService = playerNotificationService;
            _mapper = mapper;
        }

        public async Task<Result<bool>> Handle(InviteSessionCommand request, CancellationToken cancellationToken)
        {
            var sessionResult = await _accessService.GetSessionForUpdateAsync(request.SessionId, cancellationToken);

            if (sessionResult.IsFailure)
                return Result<bool>.Failure(sessionResult.Error);

            var session = sessionResult.Value;

            foreach (var idPlayer in request.PlayerIds)
            {
                await _playerNotificationService.NotifyInvitationSessionAsync(idPlayer, _mapper.Map<SessionDto>(session));
            }

            return Result<bool>.Success(true);
        }
    }
}
