using CodeBattleArena.Application.Common.Models.Dtos;

namespace CodeBattleArena.Application.Common.Interfaces.Notifications
{
    public interface IPlayerNotificationService
    {
        Task NotifyFriendRequestAsync(Guid idReceiver, PlayerDto sender);
        Task NotifyInvitationSessionAsync(Guid idPlayer, SessionDto sessionDto);
        Task NotifyItemEquippedAsync(Guid idPlayer, ItemDto item);
    }
}
