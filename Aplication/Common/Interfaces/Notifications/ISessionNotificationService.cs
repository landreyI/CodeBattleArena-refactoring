using CodeBattleArena.Application.Common.Models.Dtos;

namespace CodeBattleArena.Application.Common.Interfaces.Notifications
{
    public interface ISessionNotificationService
    {
        Task NotifySessionAddAsync(SessionDto sessionDto);
        Task NotifySessionDeletedGroupAsync(Guid id);
        Task NotifySessionDeletedAllAsync(Guid id);
        Task NotifySessionUpdatedGroupAsync(SessionDto sessionDto);
        Task NotifySessionUpdatedAllAsync(SessionDto sessionDto);
        Task NotifySessionJoinAsync(Guid id, PlayerDto player);
        Task NotifySessionUnjoinAsync(Guid id, PlayerDto player);
        Task NotifySessionKickOutAsync(Guid id, PlayerDto player);
        Task NotifyStartGameAsync(Guid id);
        Task NotifyFinishGameAsync(Guid id);
        Task NotifyUpdateCompletedCount(Guid id, int count);
        Task NotifyUpdateCodePlayerAsync(Guid id, string code);
        Task NotifyUpdatePlayerSessionAsync(PlayerSessionDto playerSession);

        //Task NotifySendMessageSessionAsync(int idSession, MessageDto messageDto);
    }
}
