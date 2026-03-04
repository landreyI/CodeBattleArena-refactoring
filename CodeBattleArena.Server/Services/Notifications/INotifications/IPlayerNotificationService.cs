using CodeBattleArena.Server.DTO.ModelsDTO;

namespace CodeBattleArena.Server.Services.Notifications.INotifications
{
    public interface IPlayerNotificationService
    {
        Task NotifyFriendRequestAsync(string idReceiver, PlayerDto sender);
        Task NotifyInvitationSessionAsync(string idPlayer, SessionDto sessionDto);
    }
}
